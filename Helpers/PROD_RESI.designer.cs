﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MARCUS.Helpers
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="dbsca")]
	public partial class PROD_RESIDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertPROD_RESI(PROD_RESI instance);
    partial void UpdatePROD_RESI(PROD_RESI instance);
    partial void DeletePROD_RESI(PROD_RESI instance);
    #endregion
		
		public PROD_RESIDataContext() : 
				base(global::MARCUS.Properties.Settings.Default.dbscaConnectionString3, mappingSource)
		{
			OnCreated();
		}
		
		public PROD_RESIDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public PROD_RESIDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public PROD_RESIDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public PROD_RESIDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<PROD_RESI> PROD_RESIs
		{
			get
			{
				return this.GetTable<PROD_RESI>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.PROD_RESI")]
	public partial class PROD_RESI : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _ID;
		
		private System.Nullable<int> _ID_AZIONE_RESO;
		
		private System.Nullable<int> _ID_DESCRIZIONE_RESO;
		
		private System.Nullable<int> _ID_DETTAGLIO_TIPO_LAVORAZIONE;
		
		private string _DETTAGLIO;
		
		private string _NUMERO_CLIENTE;
		
		private string _DESCR_ALTRO;
		
		private string _PAGINA;
		
		private string _FATTURA_SCADENZA;
		
		private System.Nullable<int> _STATO_AZIONE;
		
		private System.Nullable<System.DateTime> _DATA_AZIONE;
		
		private System.Nullable<System.DateTime> _DATA_INSERIMENTO;
		
		private System.Nullable<char> _INVIATO_ENEL;
		
		private System.Nullable<int> _ID_USER;
		
		private System.Nullable<char> _CANCELLATO;
		
		private System.Nullable<int> _CANCELLATO_DA;
		
		private System.Nullable<System.DateTime> _CANCELLATO_IL;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIDChanging(int value);
    partial void OnIDChanged();
    partial void OnID_AZIONE_RESOChanging(System.Nullable<int> value);
    partial void OnID_AZIONE_RESOChanged();
    partial void OnID_DESCRIZIONE_RESOChanging(System.Nullable<int> value);
    partial void OnID_DESCRIZIONE_RESOChanged();
    partial void OnID_DETTAGLIO_TIPO_LAVORAZIONEChanging(System.Nullable<int> value);
    partial void OnID_DETTAGLIO_TIPO_LAVORAZIONEChanged();
    partial void OnDETTAGLIOChanging(string value);
    partial void OnDETTAGLIOChanged();
    partial void OnNUMERO_CLIENTEChanging(string value);
    partial void OnNUMERO_CLIENTEChanged();
    partial void OnDESCR_ALTROChanging(string value);
    partial void OnDESCR_ALTROChanged();
    partial void OnPAGINAChanging(string value);
    partial void OnPAGINAChanged();
    partial void OnFATTURA_SCADENZAChanging(string value);
    partial void OnFATTURA_SCADENZAChanged();
    partial void OnSTATO_AZIONEChanging(System.Nullable<int> value);
    partial void OnSTATO_AZIONEChanged();
    partial void OnDATA_AZIONEChanging(System.Nullable<System.DateTime> value);
    partial void OnDATA_AZIONEChanged();
    partial void OnDATA_INSERIMENTOChanging(System.Nullable<System.DateTime> value);
    partial void OnDATA_INSERIMENTOChanged();
    partial void OnINVIATO_ENELChanging(System.Nullable<char> value);
    partial void OnINVIATO_ENELChanged();
    partial void OnID_USERChanging(System.Nullable<int> value);
    partial void OnID_USERChanged();
    partial void OnCANCELLATOChanging(System.Nullable<char> value);
    partial void OnCANCELLATOChanged();
    partial void OnCANCELLATO_DAChanging(System.Nullable<int> value);
    partial void OnCANCELLATO_DAChanged();
    partial void OnCANCELLATO_ILChanging(System.Nullable<System.DateTime> value);
    partial void OnCANCELLATO_ILChanged();
    #endregion
		
		public PROD_RESI()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL IDENTITY", IsPrimaryKey=true, IsDbGenerated=true)]
		public int ID
		{
			get
			{
				return this._ID;
			}
			set
			{
				if ((this._ID != value))
				{
					this.OnIDChanging(value);
					this.SendPropertyChanging();
					this._ID = value;
					this.SendPropertyChanged("ID");
					this.OnIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID_AZIONE_RESO", DbType="Int")]
		public System.Nullable<int> ID_AZIONE_RESO
		{
			get
			{
				return this._ID_AZIONE_RESO;
			}
			set
			{
				if ((this._ID_AZIONE_RESO != value))
				{
					this.OnID_AZIONE_RESOChanging(value);
					this.SendPropertyChanging();
					this._ID_AZIONE_RESO = value;
					this.SendPropertyChanged("ID_AZIONE_RESO");
					this.OnID_AZIONE_RESOChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID_DESCRIZIONE_RESO", DbType="Int")]
		public System.Nullable<int> ID_DESCRIZIONE_RESO
		{
			get
			{
				return this._ID_DESCRIZIONE_RESO;
			}
			set
			{
				if ((this._ID_DESCRIZIONE_RESO != value))
				{
					this.OnID_DESCRIZIONE_RESOChanging(value);
					this.SendPropertyChanging();
					this._ID_DESCRIZIONE_RESO = value;
					this.SendPropertyChanged("ID_DESCRIZIONE_RESO");
					this.OnID_DESCRIZIONE_RESOChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID_DETTAGLIO_TIPO_LAVORAZIONE", DbType="Int")]
		public System.Nullable<int> ID_DETTAGLIO_TIPO_LAVORAZIONE
		{
			get
			{
				return this._ID_DETTAGLIO_TIPO_LAVORAZIONE;
			}
			set
			{
				if ((this._ID_DETTAGLIO_TIPO_LAVORAZIONE != value))
				{
					this.OnID_DETTAGLIO_TIPO_LAVORAZIONEChanging(value);
					this.SendPropertyChanging();
					this._ID_DETTAGLIO_TIPO_LAVORAZIONE = value;
					this.SendPropertyChanged("ID_DETTAGLIO_TIPO_LAVORAZIONE");
					this.OnID_DETTAGLIO_TIPO_LAVORAZIONEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DETTAGLIO", DbType="NVarChar(50)")]
		public string DETTAGLIO
		{
			get
			{
				return this._DETTAGLIO;
			}
			set
			{
				if ((this._DETTAGLIO != value))
				{
					this.OnDETTAGLIOChanging(value);
					this.SendPropertyChanging();
					this._DETTAGLIO = value;
					this.SendPropertyChanged("DETTAGLIO");
					this.OnDETTAGLIOChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_NUMERO_CLIENTE", DbType="NVarChar(250)")]
		public string NUMERO_CLIENTE
		{
			get
			{
				return this._NUMERO_CLIENTE;
			}
			set
			{
				if ((this._NUMERO_CLIENTE != value))
				{
					this.OnNUMERO_CLIENTEChanging(value);
					this.SendPropertyChanging();
					this._NUMERO_CLIENTE = value;
					this.SendPropertyChanged("NUMERO_CLIENTE");
					this.OnNUMERO_CLIENTEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DESCR_ALTRO", DbType="NVarChar(250)")]
		public string DESCR_ALTRO
		{
			get
			{
				return this._DESCR_ALTRO;
			}
			set
			{
				if ((this._DESCR_ALTRO != value))
				{
					this.OnDESCR_ALTROChanging(value);
					this.SendPropertyChanging();
					this._DESCR_ALTRO = value;
					this.SendPropertyChanged("DESCR_ALTRO");
					this.OnDESCR_ALTROChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PAGINA", DbType="NVarChar(50)")]
		public string PAGINA
		{
			get
			{
				return this._PAGINA;
			}
			set
			{
				if ((this._PAGINA != value))
				{
					this.OnPAGINAChanging(value);
					this.SendPropertyChanging();
					this._PAGINA = value;
					this.SendPropertyChanged("PAGINA");
					this.OnPAGINAChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_FATTURA_SCADENZA", DbType="NVarChar(50)")]
		public string FATTURA_SCADENZA
		{
			get
			{
				return this._FATTURA_SCADENZA;
			}
			set
			{
				if ((this._FATTURA_SCADENZA != value))
				{
					this.OnFATTURA_SCADENZAChanging(value);
					this.SendPropertyChanging();
					this._FATTURA_SCADENZA = value;
					this.SendPropertyChanged("FATTURA_SCADENZA");
					this.OnFATTURA_SCADENZAChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_STATO_AZIONE", DbType="Int")]
		public System.Nullable<int> STATO_AZIONE
		{
			get
			{
				return this._STATO_AZIONE;
			}
			set
			{
				if ((this._STATO_AZIONE != value))
				{
					this.OnSTATO_AZIONEChanging(value);
					this.SendPropertyChanging();
					this._STATO_AZIONE = value;
					this.SendPropertyChanged("STATO_AZIONE");
					this.OnSTATO_AZIONEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DATA_AZIONE", DbType="DateTime")]
		public System.Nullable<System.DateTime> DATA_AZIONE
		{
			get
			{
				return this._DATA_AZIONE;
			}
			set
			{
				if ((this._DATA_AZIONE != value))
				{
					this.OnDATA_AZIONEChanging(value);
					this.SendPropertyChanging();
					this._DATA_AZIONE = value;
					this.SendPropertyChanged("DATA_AZIONE");
					this.OnDATA_AZIONEChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_DATA_INSERIMENTO", DbType="DateTime")]
		public System.Nullable<System.DateTime> DATA_INSERIMENTO
		{
			get
			{
				return this._DATA_INSERIMENTO;
			}
			set
			{
				if ((this._DATA_INSERIMENTO != value))
				{
					this.OnDATA_INSERIMENTOChanging(value);
					this.SendPropertyChanging();
					this._DATA_INSERIMENTO = value;
					this.SendPropertyChanged("DATA_INSERIMENTO");
					this.OnDATA_INSERIMENTOChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_INVIATO_ENEL", DbType="Char(1)")]
		public System.Nullable<char> INVIATO_ENEL
		{
			get
			{
				return this._INVIATO_ENEL;
			}
			set
			{
				if ((this._INVIATO_ENEL != value))
				{
					this.OnINVIATO_ENELChanging(value);
					this.SendPropertyChanging();
					this._INVIATO_ENEL = value;
					this.SendPropertyChanged("INVIATO_ENEL");
					this.OnINVIATO_ENELChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ID_USER", DbType="Int")]
		public System.Nullable<int> ID_USER
		{
			get
			{
				return this._ID_USER;
			}
			set
			{
				if ((this._ID_USER != value))
				{
					this.OnID_USERChanging(value);
					this.SendPropertyChanging();
					this._ID_USER = value;
					this.SendPropertyChanged("ID_USER");
					this.OnID_USERChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CANCELLATO", DbType="Char(1)")]
		public System.Nullable<char> CANCELLATO
		{
			get
			{
				return this._CANCELLATO;
			}
			set
			{
				if ((this._CANCELLATO != value))
				{
					this.OnCANCELLATOChanging(value);
					this.SendPropertyChanging();
					this._CANCELLATO = value;
					this.SendPropertyChanged("CANCELLATO");
					this.OnCANCELLATOChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CANCELLATO_DA", DbType="Int")]
		public System.Nullable<int> CANCELLATO_DA
		{
			get
			{
				return this._CANCELLATO_DA;
			}
			set
			{
				if ((this._CANCELLATO_DA != value))
				{
					this.OnCANCELLATO_DAChanging(value);
					this.SendPropertyChanging();
					this._CANCELLATO_DA = value;
					this.SendPropertyChanged("CANCELLATO_DA");
					this.OnCANCELLATO_DAChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CANCELLATO_IL", DbType="DateTime")]
		public System.Nullable<System.DateTime> CANCELLATO_IL
		{
			get
			{
				return this._CANCELLATO_IL;
			}
			set
			{
				if ((this._CANCELLATO_IL != value))
				{
					this.OnCANCELLATO_ILChanging(value);
					this.SendPropertyChanging();
					this._CANCELLATO_IL = value;
					this.SendPropertyChanged("CANCELLATO_IL");
					this.OnCANCELLATO_ILChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
