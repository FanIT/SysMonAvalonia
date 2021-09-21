using System;
using System.Globalization;

namespace DeviceIOControl.Disc.Smart
{
	/// <summary>Device DMA/UDMA modes</summary>
	public struct IdentifyDma
	{
		/// <summary>DMA type</summary>
		public enum DmaType
		{
			/// <summary>UDMA</summary>
			UDma = 0,
			/// <summary>DMA</summary>
			Dma = 1,
		}
		private readonly DmaType _type;
		private readonly SmartInfoCollection _info;
		/// <summary>Device info</summary>
		private SmartInfoCollection Info { get { return this._info; } }
		/// <summary>Type of DMA</summary>
		private DmaType Type { get { return this._type; } }
		/// <summary>DMA/UDMA structure indexes</summary>
		private static Int32[] DmaIndex = new Int32[] { 88, 63, };
		/// <summary>DMA/UDMA structure max bit pos</summary>
		private static Int32[] DmaMaxPos = new Int32[] { 5, 2, };
		/// <summary>Max DMA/UDMA mode supported</summary>
		public Int64? Max
		{
			get
			{
				Int64 pos = this.GetHighBitPos(false);
				return pos != 1 ? pos : (Int64?)null;
			}
		}
		/// <summary>DMA/UDMA mode selected</summary>
		public Int64? Selected
		{
			get
			{
				Int64 pos = this.GetHighBitPos(true);
				return this.Max.HasValue && pos != -1 ? pos : (Int64?)null;
			}
		}

		/// <summary>Create instance of DMA/UDMA info structure</summary>
		/// <param name="info">Device info</param>
		/// <param name="type">DMA/UDMA</param>
		public IdentifyDma(SmartInfoCollection info, DmaType type)
		{
			this._info = info;
			this._type = type;
		}
		private Int64 GetHighBitPos(Boolean isSelected)
		{
			return 0;
			/*Int32 index = IdentifyDma.DmaIndex[(Int32)this.Type];
			Int32 maxPos = IdentifyDma.DmaMaxPos[(Int32)this.Type];
			return Utils.HighBitPos(
				isSelected ? this.Info.SystemParams[index] >> 8 : this.Info.SystemParams[index],
				maxPos);*/
		}
		/// <summary>DMA/UDMA values as string</summary>
		/// <returns></returns>
		public override String ToString()
		{
			Int64? max = this.Max;
			Int64? selected = this.Selected;

			String result = String.Empty;
			if (max.HasValue)
			{
				result = String.Format(CultureInfo.CurrentUICulture, "Max {0} mode supported {1}{2}",
					this.Type.ToString().ToUpperInvariant(),
					max,
					max > 0 ? " and below" : String.Empty);
				if (selected.HasValue)
					result += Environment.NewLine + String.Format(CultureInfo.CurrentUICulture, "{0} mode selected {1}",
						this.Type.ToString().ToUpperInvariant(),
						selected);
			}
			return result;
		}
	}
}
