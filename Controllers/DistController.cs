using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace challenge_dotnet.Controllers
{
    [Route("api/[controller]")]
    public class DistController : Controller
    {
        public FeeData[] Fees;
        public float compare;
        
        public DistController()
        {
            Fees = LoadJson(); 
        }

        [HttpPost]
        public List<Response> PostDistData([FromBody] OrderData[] orderData)
        {
            List<Response> responses = new List<Response>();
            compare = 0;

            foreach (var order in orderData)
            {
                float otherFund = 0, total = 0;
                List<float> distNumsBirth = new List<float>();
                List<float> distNumsProp = new List<float>();
                string type = "";
                
                compare = 0;

                foreach (var orderItem in order.order_items)
                {
                    foreach (var fee in Fees)
                    {
                        if (fee.Order_Item_Type == orderItem.type && orderItem.type == "Birth Certificate") {
                            distNumsBirth = addDistributions(distNumsBirth, fee.Distributions);
                            total += processPayment(fee.Fees, orderItem).amount;
                            type = fee.Order_Item_Type;
                        }
                        
                        if (fee.Order_Item_Type == orderItem.type && orderItem.type == "Real Property Recording") {
                            distNumsProp = addDistributions(distNumsProp, fee.Distributions);
                            total += processPayment(fee.Fees, orderItem).amount;
                            type = fee.Order_Item_Type;
                        }
                    }
                }
                
                if (total != compare) {
                    otherFund = total - compare;
                }
                
                Response toAdd = new Response();

                toAdd.type = type;
                toAdd.date = order.order_date;
                toAdd.orderNumber = order.order_number;
                toAdd.distributions = new List<Distribution>();

                Distribution dist;

                for (var i=0; i<distNumsProp.Count; i++)
                {   
                    dist = new Distribution(
                        Fees[0].Distributions[i].name,
                        distNumsProp[i]
                        );

                    toAdd.distributions.Add(dist);
                }
                
                for (var i=0; i<distNumsBirth.Count; i++)
                {
                    dist = new Distribution(
                        Fees[0].Distributions[i].name,
                        distNumsBirth[i]
                    );

                    toAdd.distributions.Add(dist);
                }
                
                dist = new Distribution(
                    "Other",
                    otherFund
                    );
                
                toAdd.distributions.Add(dist);
                
                responses.Add(toAdd);
            }
            
            responses = addTotals(responses);

            return responses;

        }

        public List<Response> addTotals(List<Response> responses)
        {
            Dictionary<string, float> totals = new Dictionary<string, float>();
            
            foreach (var res in responses)
            {
                foreach (var dist in res.distributions)
                {
                    if (!totals.ContainsKey(dist.name))
                        totals.Add(dist.name, dist.amount);

                    else
                        totals[dist.name] += dist.amount;
                }
            }
            
            Response totalObj = new Response();
            totalObj.totals = totals;
            
            responses.Add(totalObj);
            
            return responses;
        }
        
        public FinalizedOrder processPayment(Fee[] feeData, OrderItem orderItem)
        {
            float per = 0, total = 0;
            int pages = orderItem.pages - 1;

            foreach (var fee in feeData)
            {
                if (fee.type == "flat")
                    total += fee.amount;

                else
                    per = fee.amount;
            }

            if (pages > 0)
            {
                total += pages * per;
            }
            
            FinalizedOrder toReturn = new FinalizedOrder();

            toReturn.id = orderItem.order_item_id;
            toReturn.type = orderItem.type;
            toReturn.amount = total;

            return toReturn;
        }

        public List<float> addDistributions(List<float> distNums, Distribution[] distributions)
        {
            for(var i=0; i<distributions.Length; i++)
            {
                if (i >= distNums.Count)
                    distNums.Add(0);
                distNums[i] += distributions[i].amount;
                compare += distributions[i].amount;
            }
            
            return distNums;
        }
        
        public FeeData[] LoadJson()
        {
            var path = "./feeData.json";
            
            if (System.IO.File.Exists(path))
            {
                var text = System.IO.File.ReadAllText(path);
                JArray success = JArray.Parse(text);
                FeeData[] feeData = new FeeData[success.Count];

                for (var i = 0; i < success.Count; i++)
                {
                    feeData[i] = new FeeData();
                    feeData[i] = success[i].ToObject<FeeData>();
                }
                return feeData;
            }

            return null;
        }
        
        public class OrderData
        {
            public string order_date { get; set; }
            public OrderItem[] order_items { get; set; }
            public string order_number { get; set; }
        }

        public class FeeData
        {
            public string Order_Item_Type { get; set; }
            public Fee[] Fees { get; set; }
            public Distribution[] Distributions { get; set; }
            
        }

        public class OrderItem
        {
            public int order_item_id { get; set; }
            public string type { get; set; }
            public int pages { get; set; }
        }

        public class FinalizedOrder
        {
            public int id { get; set; }
            public string type { get; set; }
            public float amount { get; set; }
        }

        public class Fee
        {
            public string name { get; set; }
            public float amount { get; set; }
            public string type { get; set; }
        }

        public class Distribution
        {
            public Distribution(string name, float amount)
            {
                this.name = name;
                this.amount = amount;
            }
            public string name { get; set; }
            public float amount { get; set; }
        }

        public class Response
        {
            public string orderNumber { get; set; }
            public string type { get; set; }
            public string date { get; set; }
            public List<Distribution> distributions { get; set; }
            public Dictionary<string, float> totals { get; set; }
        }
        
    }
}