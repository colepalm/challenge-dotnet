using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace challenge_dotnet.Controllers
{
    [Route("api/[controller]")]
    public class PriceController : Controller
    {
        public FeeData[] Fees;
        
        public PriceController()
        {
            Fees = LoadJson();   
        }
        
        [HttpPost]
        public List<Response> PostFeeData([FromBody]OrderData[] orderData)
        {
           List<Response> responses = new List<Response>();      
            
            foreach (var order in orderData)
            {
                int total = 0;
                List<FinalizedOrder> orderItems = new List<FinalizedOrder>();

                foreach (var orderItem in order.order_items)
                {
                    foreach (var feeData in Fees)
                    {
                        if (feeData.Order_Item_Type == orderItem.type)
                        {
                            orderItems.Add(processPayment(feeData.Fees, orderItem));
                            total += (int)orderItems[orderItems.Count - 1].amount;
                        }
                    }   
                }

                Response toAdd = new Response();

                toAdd.total = total;
                toAdd.id = order.order_number;
                toAdd.date = order.order_date;
                toAdd.orderItems = orderItems;

                responses.Add(toAdd);

            }

            
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
                total += (float)pages * (float)per;
            }
            
            FinalizedOrder toReturn = new FinalizedOrder();

            toReturn.id = orderItem.order_item_id;
            toReturn.type = orderItem.type;
            toReturn.amount = total;

            return toReturn;
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
            public string name { get; set; }
            public string amount { get; set; }
        }

        public class Response
        {
            public int total { get; set; }
            public string id { get; set; }
            public string date { get; set; }
            public List<FinalizedOrder> orderItems { get; set; }
        }
    }
}