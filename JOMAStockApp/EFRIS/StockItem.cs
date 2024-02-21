using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockApp.EFRIS
{
	public class StockItem
	{
		public string OperationType { get; set; }
		public string GoodsName { get; set; }
		public string GoodsCode { get; set; }
		public string MeasureUnit { get; set; }
		public double UnitPrice { get; set; }
		public string Currency { get; set; }
		public string CommodityCategoryId { get; set; }
		public string HaveExciseTax { get; set; }
		public int StockPrewarning { get; set; }
		public string HavePieceUnit { get; set; }

		public StockItem Build()
		{
			return this;
		}

		public StockItem SetOperationType(string operationType)
		{
			this.OperationType = operationType;
			return this;
		}

		public StockItem SetGoodsName(string goodsName)
		{
			this.GoodsName = goodsName;
			return this;
		}

		public StockItem SetGoodsCode(string goodsCode)
		{
			this.GoodsCode = goodsCode;
			return this;
		}

		public StockItem SetMeasureUnit(string measureUnit)
		{
			this.MeasureUnit = measureUnit;
			return this;
		}

		public StockItem SetUnitPrice(double unitPrice)
		{
			this.UnitPrice = unitPrice;
			return this;
		}

		public StockItem SetCurrency(string currency)
		{
			this.Currency = currency;
			return this;
		}

		public StockItem SetCommodityCategoryId(string commodityCategoryId)
		{
			this.CommodityCategoryId = commodityCategoryId;
			return this;
		}

		public StockItem SetHaveExciseTax(string haveExciseTax)
		{
			this.HaveExciseTax = haveExciseTax;
			return this;
		}

		public StockItem SetStockPrewarning(int stockPrewarning)
		{
			this.StockPrewarning = stockPrewarning;
			return this;
		}

		public StockItem SetHavePieceUnit(string havePieceUnit)
		{
			this.HavePieceUnit = havePieceUnit;
			return this;
		}
	}
}
