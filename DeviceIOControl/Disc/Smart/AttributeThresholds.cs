using DeviceIOControl.Native;

namespace DeviceIOControl.Disc.Smart
{
	/// <summary>S.M.A.R.T. attribute &amp; threshold value</summary>
	public struct AttributeThresholds
	{
		private readonly DiscApi.DRIVEATTRIBUTE _attribute;
		private readonly DiscApi.ATTRTHRESHOLD _threshold;
		/// <summary>Attribute value</summary>
		public DiscApi.DRIVEATTRIBUTE Attribute { get { return this._attribute; } }
		/// <summary>Threshold value</summary>
		public DiscApi.ATTRTHRESHOLD Threshold { get { return this._threshold; } }

		internal AttributeThresholds(DiscApi.DRIVEATTRIBUTE attribute, DiscApi.ATTRTHRESHOLD threshold)
			: this()
		{
			this._attribute = attribute;
			this._threshold = threshold;
		}
	}
}
