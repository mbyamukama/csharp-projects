using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.AppExtensions
{ 
    /// <summary>
    /// Presents the data model used to generate the receipts and used in serialization
    /// </summary>
    [Serializable]
    public class ReceiptDataModel
    {

        public int Balance { get; internal set; }
        public int Cash { get; internal set; }
        public int Discount { get; internal set; }
        public DataTable ItemsBought { get; internal set; }
        public string PayType { get; internal set; }
        public int PointsEarned { get; internal set; }
        public int PointsUsed { get; internal set; }
        public int PointsValue { get; internal set; }
        public string ReceiptNo { get; internal set; }
        public int Redeemed { get; internal set; }
        public int Total { get; internal set; }
        public int TotalPoints { get; internal set; }
        public string DateString { get; internal set; }
        public string Teller { get; internal set; }

        public ReceiptDataModel(DataTable dtItemsBought, string paytype, int cash, int redeemed, int total,
            int discount, int balance, string recNo, int pointsEarned, int pointsUsed, int totalPoints, int pointsValue, string dateString, string teller)
        {
            ItemsBought = dtItemsBought;
            PayType = paytype;
            Cash = cash;
            Redeemed = redeemed;
            Total = total;
            Discount = discount;
            Balance = balance;
            ReceiptNo = recNo;
            PointsEarned = pointsEarned;
            PointsUsed = pointsUsed;
            TotalPoints = totalPoints;
            PointsValue = pointsValue;
            DateString = dateString;
            Teller = teller;
        }

    }
}
