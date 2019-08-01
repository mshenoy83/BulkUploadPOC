using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.EntityClient;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

using Dapper;
using Dapper.Contrib.Extensions;

using Microsoft.Samples.EntityDataReader;

using SqlBulkTools;

namespace BulkUploadPOC
{
    public class DapperRepository<T> where T : BulkTest
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1,1);
        public string GetConnectionString => _strConnection;

        #region Fields

        private readonly string _strConnection;
        #endregion

        #region Ctor

        private readonly ReflectionCache _cache;
        /// <summary>
        /// Ctor
        /// </summary>
        public DapperRepository(ReflectionCache cache)
        {
            _cache = cache;
            _strConnection = ConfigurationManager.ConnectionStrings["SampleDbContext"].ConnectionString;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public T GetById(object id)
        {
            using (IDbConnection conn = new SqlConnection(_strConnection))
            {
                try
                {
                    conn.Open();
                    return conn.Get<T>(id);
                }
                catch (Exception e)
                {

                    throw;
                }
                finally
                {
                    conn.Close();
                }

            }
        }




        #endregion

        #region Interface Methods



        public IQueryable<T> GetListFromQuery()
        {
            using (IDbConnection conn = new SqlConnection(_strConnection))
            {
                try
                {
                    conn.Open();
                    return conn.Query<T>($"select * from {GetTableName(typeof(T))}").AsQueryable();
                }
                catch (Exception e)
                {

                    throw;
                }
                finally
                {
                    conn.Close();
                }

            }
        }

        public IEnumerable<T1> GetListFromQuery<T1>(string sql) where T1 : class, new()
        {
            using (IDbConnection conn = new SqlConnection(_strConnection))
            {
                try
                {
                    conn.Open();
                    return conn.Query<T1>(sql);
                }
                catch (Exception e)
                {


                    throw;
                }
                finally
                {
                    conn.Close();
                }

            }
        }



        public List<T> ExecuteSql(string sql, object parameter)
        {
            using (IDbConnection conn = new SqlConnection(_strConnection))
            {
                try
                {
                    conn.Open();
                    return conn.Query<T>(sql, parameter).ToList();
                }
                catch (Exception e)
                {

                    throw;
                }
                finally
                {
                    conn.Close();
                }

            }
        }


        /// <summary>
        /// Bulk Insert large number of entities
        /// </summary>
        /// <param name="entities"></param>
        public void BulkInsert(List<T> entities)
        {
            try
            {
               // _semaphore.Wait();
                var sw = new Stopwatch();
                sw.Start();
                
                using (var conn = new SqlConnection(_strConnection))
                {
                    try
                    {
                        var bulk = new BulkOperations();
                        bulk.Setup<T>(x => x.ForCollection(entities))
                            .WithTable("BulkTest")
                            .WithSqlBulkCopyOptions(SqlBulkCopyOptions.TableLock)
                            .AddAllColumns()
                            .BulkInsert();
                        bulk.CommitTransaction(conn);
                        //conn.Open();

                        //using (SqlTransaction transaction = conn.BeginTransaction())
                        //using (var dReader = new EntityDataReader<T>(entities))
                        //{
                        //    using (var bulkcopy = new SqlBulkCopy(_strConnection, SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.UseInternalTransaction))
                        //    {
                        //        bulkcopy.EnableStreaming = true;
                        //        bulkcopy.DestinationTableName = "BulkTest";
                        //        var Props = _cache.GetObjectProperties("BulkTest");
                        //        foreach (PropertyInfo prop in Props)
                        //        {
                        //            //Setting column names as Property names  
                        //            bulkcopy.ColumnMappings.Add(prop.Name, prop.Name);
                        //        }
                        //        bulkcopy.BulkCopyTimeout = Int32.MaxValue;
                        //        bulkcopy.WriteToServer(dReader);
                        //        //transaction.Commit();
                        //        bulkcopy.Close();
                        //    }
                        //}
                    }
                    catch (Exception e)
                    {
                        throw;
                    }
                    finally
                    {
                        conn.Close();
                    }

                }
                sw.Stop();
                Logger.Log.Information("Time taken for Bulk Insert = {0}. Items : {0}", sw.ElapsedMilliseconds, entities.Count);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
            finally
            {
               // _semaphore.Release();
            }
           
        }

        public void BulkInsert(String strQuery, object parameter)
        {
            using (var conn = new SqlConnection(_strConnection))
            {
                try
                {

                    conn.Open();

                    using (SqlTransaction transaction = conn.BeginTransaction())
                    using (var dReader = conn.ExecuteReader(strQuery, parameter, transaction))
                    {
                        using (var bulkcopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.TableLock, transaction))
                        {
                            bulkcopy.EnableStreaming = true;
                            bulkcopy.DestinationTableName = "BulkTest";
                            var Props = _cache.GetObjectProperties("BulkTest");
                            foreach (PropertyInfo prop in Props)
                            {
                                //Setting column names as Property names  
                                bulkcopy.ColumnMappings.Add(prop.Name, prop.Name);
                            }
                            bulkcopy.BulkCopyTimeout = Int32.MaxValue;
                            bulkcopy.WriteToServer(dReader);
                            transaction.Commit();
                            bulkcopy.Close();
                        }
                    }
                    //var bulk = new BulkOperations();
                    //bulk.Setup<T>(x => x.ForCollection(entities))
                    //    .WithTable("BulkTest")
                    //    .WithSqlBulkCopyOptions(SqlBulkCopyOptions.TableLock)
                    //    //.WithBulkCopyBatchSize(100000)
                    //    .AddAllColumns()
                    //    .BulkInsert();
                    //bulk.CommitTransaction(conn);
                }
                catch (Exception e)
                {
                    throw;
                }
                finally
                {
                    conn.Close();
                }

            }
        }





        public T1 ExecuteScalar<T1>(string sql, object parameter)
        {
            using (IDbConnection conn = new SqlConnection(_strConnection))
            {
                try
                {
                    conn.Open();
                    return conn.ExecuteScalar<T1>(sql, parameter);
                }
                catch (Exception e)
                {


                    throw;
                }
                finally
                {
                    conn.Close();
                }

            }
        }


        #endregion

        #region Private Methods

        private string GetTableName(Type entityType)
        {
            if (entityType.CustomAttributes.Any())
            {
                return entityType.CustomAttributes.ToList()[0].ConstructorArguments[0].Value.ToString();
            }
            return entityType.Name;
        }
        #endregion
    }
}
