using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using SqlBulkTools;

namespace BulkUploadPOC
{
    class Program
    {
        static void Main(string[] args)
        {
            var cache = new ReflectionCache();
            var swttl = new Stopwatch();
            swttl.Start();
            var repo = new DapperRepository<BulkTest>(cache);
            var maxCount = repo.ExecuteScalar<int>("Select Max(Id) from SalesRecords_Test", null);
            var quotient = maxCount / 100000;
            var remainder = maxCount % 100000;
            if (remainder > 1)
            {
                quotient += 1;
            }

            string query = "Select * from SalesRecords_Test Where Id between @aParam and @bParam";
            //var rng = Enumerable.Range(1, quotient).ToList().GroupBy(n => (n - 1) / 10).ToList();

            //Parallel.ForEach(rng, grp =>
            //{
            //    var prrllList = new ConcurrentBag<BulkTest>();
            //    Parallel.ForEach(grp, i =>
            //    {
            //        var myrepo = new DapperRepository<BulkTest>(cache);
            //        var fromValue = (i - 1) * 100000 + 1;
            //        var toValue = i * 100000;
            //        var sw = new Stopwatch();
            //        sw.Start();
            //        repo.BulkInsert(query,new { aParam = fromValue, bParam = toValue });
            //        sw.Stop();
            //        Logger.Log.Information("Iteration {0}.Time taken for Bulk Insert = {1}. Items : {2}", grp.Key, sw.ElapsedMilliseconds, prrllList.Count);
            //        //var stagingData = myrepo.ExecuteSql(query, new { aParam = fromValue, bParam = toValue });
            //        //var unqStagingData = stagingData
            //        //                     .GroupBy(x => new
            //        //                     {
            //        //                         x.Country,
            //        //                         x.Item_Type,
            //        //                         x.Order_Date,
            //        //                         x.Order_ID,
            //        //                         x.Order_Priority,
            //        //                         x.Region,
            //        //                         x.Sales_Channel,
            //        //                         x.Ship_Date,
            //        //                         x.Total_Cost,
            //        //                         x.Total_Profit
            //        //                     }).Select(c => c.FirstOrDefault()).ToList();
            //        //var newItems = unqStagingData.Select(x => new BulkTest
            //        //{
            //        //    Country = x.Country,
            //        //    Order_ID = x.Order_ID,
            //        //    Sales_Channel = x.Sales_Channel,
            //        //    Total_Revenue = x.Total_Revenue,
            //        //    Item_Type = x.Item_Type,
            //        //    Ship_Date = x.Ship_Date,
            //        //    Order_Date = x.Order_Date,
            //        //    Total_Profit = x.Total_Profit,
            //        //    Order_Priority = x.Order_Priority,
            //        //    Region = x.Region,
            //        //    Total_Cost = x.Total_Cost,
            //        //    Unit_Cost = x.Unit_Cost,
            //        //    Unit_Price = x.Unit_Price,
            //        //    Units_Sold = x.Units_Sold
            //        //}).ToList();
            //        //foreach (var items in newItems)
            //        //{
            //        //    prrllList.Add(items);
            //        //}
            //    });

            //});
            ThreadLocal<Stopwatch> _localStopwatch = new ThreadLocal<Stopwatch>(() => new Stopwatch());
            Parallel.For(1, quotient + 1, i =>
                   {
                       var myrepo = new DapperRepository<BulkTest>(cache);
                      
                       var fromValue = (i - 1) * 100000 + 1;
                       var toValue = i * 100000;

                       var stagingData = myrepo.ExecuteSql(query, new { aParam = fromValue, bParam = toValue });

                       var newItems = stagingData.Select(x => new BulkTest
                       {
                           Country = x.Country,
                           Order_ID = x.Order_ID,
                           Sales_Channel = x.Sales_Channel,
                           Total_Revenue = x.Total_Revenue,
                           Item_Type = x.Item_Type,
                           Ship_Date = x.Ship_Date,
                           Order_Date = x.Order_Date,
                           Total_Profit = x.Total_Profit,
                           Order_Priority = x.Order_Priority,
                           Region = x.Region,
                           Total_Cost = x.Total_Cost,
                           Unit_Cost = x.Unit_Cost,
                           Unit_Price = x.Unit_Price,
                           Units_Sold = x.Units_Sold
                       }).ToList();

                       if (newItems.Any())
                       {
                           _localStopwatch.Value.Start();
                           myrepo.BulkInsert(newItems);
                           _localStopwatch.Value.Stop();
                           Logger.Log.Information("Total Time taken BulkInsert {0}", _localStopwatch.Value.ElapsedMilliseconds);
                       }
                   });
            swttl.Stop();
            Logger.Log.Information("Total Time taken {0}", swttl.ElapsedMilliseconds);
        }
    }
}
