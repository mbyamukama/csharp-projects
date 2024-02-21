using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.EFRIS
{
	public class GoodsUploadRequest
	{
		public string OperationType { get; set; }
		public string GoodsName { get; set; }
		public string GoodsCode { get; set; }
		public string MeasureUnit { get; set; }
		public string UnitPrice { get; set; }
		public string Currency { get; set; }
		public string CommodityCategoryId { get; set; }
		public string HaveExciseTax { get; set; }
		public string Description { get; set; }
		public string StockPrewarning { get; set; }
		public string PieceMeasureUnit { get; set; }
		public string HavePieceUnit { get; set; }
		public string PieceUnitPrice { get; set; }
		public string PackageScaledValue { get; set; }
		public string PieceScaledValue { get; set; }
		public string ExciseDutyCode { get; set; }
		public string HaveOtherUnit { get; set; }
		public string GoodsTypeCode { get; set; }
		public List<GoodsOtherUnit> GoodsOtherUnits { get; set; }
	}

	public class GoodsOtherUnit
	{
		public string OtherUnit { get; set; }
		public string OtherPrice { get; set; }
		public string OtherScaled { get; set; }
		public string PackageScaled { get; set; }
	}

}
