using System;
using DeviceIOControl.Native;

namespace DeviceIOControl.Storage
{
	/// <summary>Device properties</summary>
	public class Properties
	{
		private readonly DeviceIoControl _info;
		private StorageApi.STORAGE_DEVICE_DESCRIPTOR? _device;
		private StorageApi.STORAGE_ADAPTER_DESCRIPTOR? _adapter;
		private StorageApi.STORAGE_DEVICE_ID_DESCRIPTOR? _id;
		private StorageApi.STORAGE_WRITE_CACHE_PROPERTY? _writeCache;
		private StorageApi.STORAGE_MINIPORT_DESCRIPTOR? _miniport;
		private StorageApi.STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR? _accessAlignment;
		private StorageApi.DEVICE_SEEK_PENALTY_DESCRIPTOR? _seekPenalty;
		private StorageApi.DEVICE_TRIM_DESCRIPTOR? _trim;
		private StorageApi.DEVICE_WRITE_AGGREGATION_DESCRIPTOR? _writeAggregation;
		private StorageApi.DEVICE_POWER_DESCRIPTOR? _power;
		private StorageApi.DEVICE_COPY_OFFLOAD_DESCRIPTOR? _copyOffload;
		private StorageApi.STORAGE_DEVICE_RESILIENCY_DESCRIPTOR? _resiliency;

		/// <summary>Device</summary>
		private DeviceIoControl Info { get { return this._info; } }

		/// <summary>Device property descriptor</summary>
		public StorageApi.STORAGE_DEVICE_DESCRIPTOR? Device
		{
			get
			{
				if (this._device == null)
					this._device = this.IoControl<StorageApi.STORAGE_DEVICE_DESCRIPTOR>();

				return this._device.Value.Size == 0
					? (StorageApi.STORAGE_DEVICE_DESCRIPTOR?)null
					: this._device.Value;
			}
		}
		/// <summary>Storage adapter descriptor data for a device.</summary>
		public StorageApi.STORAGE_ADAPTER_DESCRIPTOR? Adapter
		{
			get
			{
				if (this._adapter == null)
					this._adapter = this.IoControl<StorageApi.STORAGE_ADAPTER_DESCRIPTOR>();

				return this._adapter.Value.Size == 0
					? (StorageApi.STORAGE_ADAPTER_DESCRIPTOR?)null
					: this._adapter.Value;
			}
		}
		/// <summary>Device ID descriptor data for a device.</summary>
		public StorageApi.STORAGE_DEVICE_ID_DESCRIPTOR? Id
		{
			get
			{
				if (this._id == null)
					this._id = this.IoControl<StorageApi.STORAGE_DEVICE_ID_DESCRIPTOR>();

				return this._id.Value.Size == 0
					? (StorageApi.STORAGE_DEVICE_ID_DESCRIPTOR?)null
					: this._id.Value;
			}
		}
		/// <summary>Device write cache property</summary>
		public StorageApi.STORAGE_WRITE_CACHE_PROPERTY? WriteCache
		{
			get
			{
				if (this._writeCache == null)
					this._writeCache = this.IoControl<StorageApi.STORAGE_WRITE_CACHE_PROPERTY>();

				return this._writeCache.Value.Size == 0
					? (StorageApi.STORAGE_WRITE_CACHE_PROPERTY?)null
					: this._writeCache.Value;
			}
		}
		/// <summary>Storage adapter miniport driver descriptor data for a device</summary>
		public StorageApi.STORAGE_MINIPORT_DESCRIPTOR? Miniport
		{
			get
			{
				if (this._miniport == null)
					this._miniport = this.IoControl<StorageApi.STORAGE_MINIPORT_DESCRIPTOR>();

				return this._miniport.Value.Size == 0
					? (StorageApi.STORAGE_MINIPORT_DESCRIPTOR?)null
					: this._miniport.Value;
			}
		}
		/// <summary>Storage access alignment descriptor data for a device</summary>
		public StorageApi.STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR? AccessAlignment
		{
			get
			{
				if (this._accessAlignment == null)
					this._accessAlignment = this.IoControl<StorageApi.STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR>();

				return this._accessAlignment.Value.Size == 0
					? (StorageApi.STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR?)null
					: this._accessAlignment.Value;
			}
		}
		/// <summary>Seek penalty descriptor data for a device</summary>
		public StorageApi.DEVICE_SEEK_PENALTY_DESCRIPTOR? SeekPenalty
		{
			get
			{
				if (this._seekPenalty == null)
					this._seekPenalty = this.IoControl<StorageApi.DEVICE_SEEK_PENALTY_DESCRIPTOR>();

				return this._seekPenalty.Value.Size == 0
					? (StorageApi.DEVICE_SEEK_PENALTY_DESCRIPTOR?)null
					: this._seekPenalty.Value;
			}
		}
		/// <summary>Trim descriptor data for a device</summary>
		public StorageApi.DEVICE_TRIM_DESCRIPTOR? Trim
		{
			get
			{
				if (this._trim == null)
					this._trim = this.IoControl<StorageApi.DEVICE_TRIM_DESCRIPTOR>();

				return this._trim.Value.Size == 0
					? (StorageApi.DEVICE_TRIM_DESCRIPTOR?)null
					: this._trim.Value;
			}
		}
		/// <summary>Write aggregation data for a device</summary>
		public StorageApi.DEVICE_WRITE_AGGREGATION_DESCRIPTOR? WriteAggregation
		{
			get
			{
				if (this._writeAggregation == null)
					this._writeAggregation = this.IoControl<StorageApi.DEVICE_WRITE_AGGREGATION_DESCRIPTOR>();

				return this._writeAggregation.Value.Size == 0
					? (StorageApi.DEVICE_WRITE_AGGREGATION_DESCRIPTOR?)null
					: this._writeAggregation.Value;
			}
		}
		/// <summary>Describes the power capabilities of a storage device</summary>
		public StorageApi.DEVICE_POWER_DESCRIPTOR? Power
		{
			get
			{
				if (this._power == null)
					this._power = this.IoControl<StorageApi.DEVICE_POWER_DESCRIPTOR>();

				return this._power.Value.Size == 0
					? (StorageApi.DEVICE_POWER_DESCRIPTOR?)null
					: this._power.Value;
			}
		}
		/// <summary>Copy offload capabilities for a storage device.</summary>
		public StorageApi.DEVICE_COPY_OFFLOAD_DESCRIPTOR? CopyOffload
		{
			get
			{
				if (this._copyOffload == null)
					this._copyOffload = this.IoControl<StorageApi.DEVICE_COPY_OFFLOAD_DESCRIPTOR>();

				return this._copyOffload.Value.Size == 0
					? (StorageApi.DEVICE_COPY_OFFLOAD_DESCRIPTOR?)null
					: this._copyOffload.Value;
			}
		}

		/// <summary>Resiliency capabilities for a storage device.</summary>
		public StorageApi.STORAGE_DEVICE_RESILIENCY_DESCRIPTOR? Resiliency
		{
			get
			{
				if (this._resiliency == null)
					this._resiliency = this.IoControl<StorageApi.STORAGE_DEVICE_RESILIENCY_DESCRIPTOR>();

				return this._resiliency.Value.Size == 0
					? (StorageApi.STORAGE_DEVICE_RESILIENCY_DESCRIPTOR?)null
					: this._resiliency.Value;
			}
		}

		/// <summary>Create instance of device properties class</summary>
		/// <param name="device">device info</param>
		internal Properties(DeviceIoControl device)
		{
			this._info = device;
		}

		/// <summary>Проверка на существование свойства перед запросом</summary>
		/// <param name="type">Тип возвращаемого обйекта</param>
		/// <returns>Свойство существует</returns>
		private Boolean IsPropertyExists(Type type)
		{
			return this.IsPropertyExists(Properties.GetPropertyId(type));
		}

		/// <summary>Проверка на существование свойства перед запросом</summary>
		/// <param name="propertyId">Идентификатор свойства перед запросом</param>
		/// <returns>Такое свойство существует</returns>
		private Boolean IsPropertyExists(StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID propertyId)
		{
			StorageApi.STORAGE_PROPERTY_QUERY query = new StorageApi.STORAGE_PROPERTY_QUERY();
			query.QueryType = StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_QUERY_TYPE.PropertyExistsQuery;
			query.PropertyId = propertyId;

			return this.Info.IoControl(Constant.IOCTL_STORAGE.QUERY_PROPERTY, query);
		}

		private T IoControl<T>() where T : struct
		{
			StorageApi.STORAGE_PROPERTY_QUERY inParams = new StorageApi.STORAGE_PROPERTY_QUERY();
			inParams.QueryType = StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_QUERY_TYPE.PropertyStandardQuery;
			inParams.PropertyId = Properties.GetPropertyId(typeof(T));

			T outParams;
			if (this.IsPropertyExists(inParams.PropertyId)//Перед запросом проверяю поддержку этого свойства устройством
				&& this.Info.IoControl<T>(Constant.IOCTL_STORAGE.QUERY_PROPERTY, inParams, out outParams))
				return outParams;
			else
				return default(T);
		}

		private static StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID GetPropertyId(Type type)
		{
			if (type == typeof(StorageApi.STORAGE_DEVICE_DESCRIPTOR))
				return StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageDeviceProperty;
			else if (type == typeof(StorageApi.STORAGE_ADAPTER_DESCRIPTOR))
				return StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageAdapterProperty;
			else if (type == typeof(StorageApi.STORAGE_DEVICE_ID_DESCRIPTOR))
				return StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageDeviceIdProperty;
			/*else if(type == typeof(StorageWinAPI.STORAGE_DEVICE_UNIQUE_IDENTIFIER))
				query.PropertyId = StorageWinAPI.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageDeviceUniqueIdProperty;*/
			else if (type == typeof(StorageApi.STORAGE_WRITE_CACHE_PROPERTY))
				return StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageDeviceWriteCacheProperty;
			else if (type == typeof(StorageApi.STORAGE_MINIPORT_DESCRIPTOR))
				return StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageMiniportProperty;
			else if (type == typeof(StorageApi.STORAGE_ACCESS_ALIGNMENT_DESCRIPTOR))
				return StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageAccessAlignmentProperty;
			else if (type == typeof(StorageApi.DEVICE_SEEK_PENALTY_DESCRIPTOR))
				return StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageDeviceSeekPenaltyProperty;
			else if (type == typeof(StorageApi.DEVICE_TRIM_DESCRIPTOR))
				return StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageDeviceTrimProperty;
			else if (type == typeof(StorageApi.DEVICE_WRITE_AGGREGATION_DESCRIPTOR))
				return StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageDeviceWriteAggregationProperty;
			/*else if(type == typeof(WinNT.DEVICE_LB_PROVISIONING_DESCRIPTOR))
				return  StorageWinAPI.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageDeviceLBProvisioningProperty;*/
			else if (type == typeof(StorageApi.DEVICE_POWER_DESCRIPTOR))
				return StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageDeviceZeroPowerProperty;
			else if (type == typeof(StorageApi.DEVICE_COPY_OFFLOAD_DESCRIPTOR))
				return StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageDeviceCopyOffloadProperty;
			else if (type == typeof(StorageApi.STORAGE_DEVICE_RESILIENCY_DESCRIPTOR))
				return StorageApi.STORAGE_PROPERTY_QUERY.STORAGE_PROPERTY_ID.StorageDeviceResiliencyProperty;
			else
				throw new NotImplementedException();
		}
	}
}
