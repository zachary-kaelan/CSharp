﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TaxRatesInKansas.OhioWebService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", ConfigurationName="OhioWebService.OHFinderServiceSoap")]
    public interface OHFinderServiceSoap {
        
        // CODEGEN: Generating message contract since the operation GetOHSalesTaxByAddress is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="https://thefinder.tax.ohio.gov/OHFinderService/GetOHSalesTaxByAddress", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressResponse GetOHSalesTaxByAddress(TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="https://thefinder.tax.ohio.gov/OHFinderService/GetOHSalesTaxByAddress", ReplyAction="*")]
        System.Threading.Tasks.Task<TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressResponse> GetOHSalesTaxByAddressAsync(TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressRequest request);
        
        // CODEGEN: Generating message contract since the operation GetOHSalesTaxByZipCode is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="https://thefinder.tax.ohio.gov/OHFinderService/GetOHSalesTaxByZipCode", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeResponse GetOHSalesTaxByZipCode(TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="https://thefinder.tax.ohio.gov/OHFinderService/GetOHSalesTaxByZipCode", ReplyAction="*")]
        System.Threading.Tasks.Task<TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeResponse> GetOHSalesTaxByZipCodeAsync(TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeRequest request);
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/")]
    public partial class AddressReturn : object, System.ComponentModel.INotifyPropertyChanged {
        
        private AddressResponse addressResponseField;
        
        private TaxResponse[] taxResponseField;
        
        private int resultCodeField;
        
        private string resultField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public AddressResponse addressResponse {
            get {
                return this.addressResponseField;
            }
            set {
                this.addressResponseField = value;
                this.RaisePropertyChanged("addressResponse");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Order=1)]
        public TaxResponse[] taxResponse {
            get {
                return this.taxResponseField;
            }
            set {
                this.taxResponseField = value;
                this.RaisePropertyChanged("taxResponse");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public int resultCode {
            get {
                return this.resultCodeField;
            }
            set {
                this.resultCodeField = value;
                this.RaisePropertyChanged("resultCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string result {
            get {
                return this.resultField;
            }
            set {
                this.resultField = value;
                this.RaisePropertyChanged("result");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/")]
    public partial class AddressResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string foundStreetAddressField;
        
        private string foundAddressLine2Field;
        
        private string foundCityField;
        
        private string foundStateField;
        
        private string foundZipCodeField;
        
        private string foundZipPlus4Field;
        
        private string foundPhysicalCityField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string FoundStreetAddress {
            get {
                return this.foundStreetAddressField;
            }
            set {
                this.foundStreetAddressField = value;
                this.RaisePropertyChanged("FoundStreetAddress");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string FoundAddressLine2 {
            get {
                return this.foundAddressLine2Field;
            }
            set {
                this.foundAddressLine2Field = value;
                this.RaisePropertyChanged("FoundAddressLine2");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string FoundCity {
            get {
                return this.foundCityField;
            }
            set {
                this.foundCityField = value;
                this.RaisePropertyChanged("FoundCity");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string FoundState {
            get {
                return this.foundStateField;
            }
            set {
                this.foundStateField = value;
                this.RaisePropertyChanged("FoundState");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string FoundZipCode {
            get {
                return this.foundZipCodeField;
            }
            set {
                this.foundZipCodeField = value;
                this.RaisePropertyChanged("FoundZipCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string FoundZipPlus4 {
            get {
                return this.foundZipPlus4Field;
            }
            set {
                this.foundZipPlus4Field = value;
                this.RaisePropertyChanged("FoundZipPlus4");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public string FoundPhysicalCity {
            get {
                return this.foundPhysicalCityField;
            }
            set {
                this.foundPhysicalCityField = value;
                this.RaisePropertyChanged("FoundPhysicalCity");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/")]
    public partial class ZipResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string foundZipCodeField;
        
        private string foundZipPlus4Field;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string FoundZipCode {
            get {
                return this.foundZipCodeField;
            }
            set {
                this.foundZipCodeField = value;
                this.RaisePropertyChanged("FoundZipCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string FoundZipPlus4 {
            get {
                return this.foundZipPlus4Field;
            }
            set {
                this.foundZipPlus4Field = value;
                this.RaisePropertyChanged("FoundZipPlus4");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/")]
    public partial class ZipCodeReturn : object, System.ComponentModel.INotifyPropertyChanged {
        
        private ZipResponse zipResponseField;
        
        private TaxResponse[] taxResponseField;
        
        private int resultCodeField;
        
        private string resultField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public ZipResponse zipResponse {
            get {
                return this.zipResponseField;
            }
            set {
                this.zipResponseField = value;
                this.RaisePropertyChanged("zipResponse");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlArrayAttribute(Order=1)]
        public TaxResponse[] taxResponse {
            get {
                return this.taxResponseField;
            }
            set {
                this.taxResponseField = value;
                this.RaisePropertyChanged("taxResponse");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public int resultCode {
            get {
                return this.resultCodeField;
            }
            set {
                this.resultCodeField = value;
                this.RaisePropertyChanged("resultCode");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string result {
            get {
                return this.resultField;
            }
            set {
                this.resultField = value;
                this.RaisePropertyChanged("result");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "4.7.2612.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/")]
    public partial class TaxResponse : object, System.ComponentModel.INotifyPropertyChanged {
        
        private string stateNameField;
        
        private string stateFipsField;
        
        private string countyNameField;
        
        private string countyFipsField;
        
        private string transitNameField;
        
        private string transitFipsField;
        
        private double generalTaxIntrastateField;
        
        private double generalTaxInterstateField;
        
        private double foodDrugTaxIntrastateField;
        
        private double foodDrugTaxInterstateField;
        
        private double totalSalesTaxAmountField;
        
        private double totalSalesAmountWithTaxField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=0)]
        public string StateName {
            get {
                return this.stateNameField;
            }
            set {
                this.stateNameField = value;
                this.RaisePropertyChanged("StateName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=1)]
        public string StateFips {
            get {
                return this.stateFipsField;
            }
            set {
                this.stateFipsField = value;
                this.RaisePropertyChanged("StateFips");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=2)]
        public string CountyName {
            get {
                return this.countyNameField;
            }
            set {
                this.countyNameField = value;
                this.RaisePropertyChanged("CountyName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=3)]
        public string CountyFips {
            get {
                return this.countyFipsField;
            }
            set {
                this.countyFipsField = value;
                this.RaisePropertyChanged("CountyFips");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=4)]
        public string TransitName {
            get {
                return this.transitNameField;
            }
            set {
                this.transitNameField = value;
                this.RaisePropertyChanged("TransitName");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=5)]
        public string TransitFips {
            get {
                return this.transitFipsField;
            }
            set {
                this.transitFipsField = value;
                this.RaisePropertyChanged("TransitFips");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=6)]
        public double GeneralTaxIntrastate {
            get {
                return this.generalTaxIntrastateField;
            }
            set {
                this.generalTaxIntrastateField = value;
                this.RaisePropertyChanged("GeneralTaxIntrastate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=7)]
        public double GeneralTaxInterstate {
            get {
                return this.generalTaxInterstateField;
            }
            set {
                this.generalTaxInterstateField = value;
                this.RaisePropertyChanged("GeneralTaxInterstate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=8)]
        public double FoodDrugTaxIntrastate {
            get {
                return this.foodDrugTaxIntrastateField;
            }
            set {
                this.foodDrugTaxIntrastateField = value;
                this.RaisePropertyChanged("FoodDrugTaxIntrastate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=9)]
        public double FoodDrugTaxInterstate {
            get {
                return this.foodDrugTaxInterstateField;
            }
            set {
                this.foodDrugTaxInterstateField = value;
                this.RaisePropertyChanged("FoodDrugTaxInterstate");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=10)]
        public double TotalSalesTaxAmount {
            get {
                return this.totalSalesTaxAmountField;
            }
            set {
                this.totalSalesTaxAmountField = value;
                this.RaisePropertyChanged("TotalSalesTaxAmount");
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Order=11)]
        public double TotalSalesAmountWithTax {
            get {
                return this.totalSalesAmountWithTaxField;
            }
            set {
                this.totalSalesAmountWithTaxField = value;
                this.RaisePropertyChanged("TotalSalesAmountWithTax");
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetOHSalesTaxByAddressRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=0)]
        public string AddressLine;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=1)]
        public string City;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=2)]
        public string StateOrProvince;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=3)]
        public string PostalCode;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=4)]
        public string CountryCode;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=5)]
        public double SalesAmount;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=6)]
        public System.DateTime SalesDate;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=7)]
        public bool ReturnMultiple;
        
        public GetOHSalesTaxByAddressRequest() {
        }
        
        public GetOHSalesTaxByAddressRequest(string AddressLine, string City, string StateOrProvince, string PostalCode, string CountryCode, double SalesAmount, System.DateTime SalesDate, bool ReturnMultiple) {
            this.AddressLine = AddressLine;
            this.City = City;
            this.StateOrProvince = StateOrProvince;
            this.PostalCode = PostalCode;
            this.CountryCode = CountryCode;
            this.SalesAmount = SalesAmount;
            this.SalesDate = SalesDate;
            this.ReturnMultiple = ReturnMultiple;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetOHSalesTaxByAddressResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=0)]
        public TaxRatesInKansas.OhioWebService.AddressReturn GetOHSalesTaxByAddressResult;
        
        public GetOHSalesTaxByAddressResponse() {
        }
        
        public GetOHSalesTaxByAddressResponse(TaxRatesInKansas.OhioWebService.AddressReturn GetOHSalesTaxByAddressResult) {
            this.GetOHSalesTaxByAddressResult = GetOHSalesTaxByAddressResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetOHSalesTaxByZipCodeRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=0)]
        public string PostalCode;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=1)]
        public double SalesAmount;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=2)]
        public System.DateTime SalesDate;
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=3)]
        public bool ReturnMultiple;
        
        public GetOHSalesTaxByZipCodeRequest() {
        }
        
        public GetOHSalesTaxByZipCodeRequest(string PostalCode, double SalesAmount, System.DateTime SalesDate, bool ReturnMultiple) {
            this.PostalCode = PostalCode;
            this.SalesAmount = SalesAmount;
            this.SalesDate = SalesDate;
            this.ReturnMultiple = ReturnMultiple;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class GetOHSalesTaxByZipCodeResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="https://thefinder.tax.ohio.gov/OHFinderService/", Order=0)]
        public TaxRatesInKansas.OhioWebService.ZipCodeReturn GetOHSalesTaxByZipCodeResult;
        
        public GetOHSalesTaxByZipCodeResponse() {
        }
        
        public GetOHSalesTaxByZipCodeResponse(TaxRatesInKansas.OhioWebService.ZipCodeReturn GetOHSalesTaxByZipCodeResult) {
            this.GetOHSalesTaxByZipCodeResult = GetOHSalesTaxByZipCodeResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface OHFinderServiceSoapChannel : TaxRatesInKansas.OhioWebService.OHFinderServiceSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class OHFinderServiceSoapClient : System.ServiceModel.ClientBase<TaxRatesInKansas.OhioWebService.OHFinderServiceSoap>, TaxRatesInKansas.OhioWebService.OHFinderServiceSoap {
        
        public OHFinderServiceSoapClient() {
        }
        
        public OHFinderServiceSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public OHFinderServiceSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OHFinderServiceSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public OHFinderServiceSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressResponse TaxRatesInKansas.OhioWebService.OHFinderServiceSoap.GetOHSalesTaxByAddress(TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressRequest request) {
            return base.Channel.GetOHSalesTaxByAddress(request);
        }
        
        public TaxRatesInKansas.OhioWebService.AddressReturn GetOHSalesTaxByAddress(string AddressLine, string City, string StateOrProvince, string PostalCode, string CountryCode, double SalesAmount, System.DateTime SalesDate, bool ReturnMultiple) {
            TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressRequest inValue = new TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressRequest();
            inValue.AddressLine = AddressLine;
            inValue.City = City;
            inValue.StateOrProvince = StateOrProvince;
            inValue.PostalCode = PostalCode;
            inValue.CountryCode = CountryCode;
            inValue.SalesAmount = SalesAmount;
            inValue.SalesDate = SalesDate;
            inValue.ReturnMultiple = ReturnMultiple;
            TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressResponse retVal = ((TaxRatesInKansas.OhioWebService.OHFinderServiceSoap)(this)).GetOHSalesTaxByAddress(inValue);
            return retVal.GetOHSalesTaxByAddressResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressResponse> TaxRatesInKansas.OhioWebService.OHFinderServiceSoap.GetOHSalesTaxByAddressAsync(TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressRequest request) {
            return base.Channel.GetOHSalesTaxByAddressAsync(request);
        }
        
        public System.Threading.Tasks.Task<TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressResponse> GetOHSalesTaxByAddressAsync(string AddressLine, string City, string StateOrProvince, string PostalCode, string CountryCode, double SalesAmount, System.DateTime SalesDate, bool ReturnMultiple) {
            TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressRequest inValue = new TaxRatesInKansas.OhioWebService.GetOHSalesTaxByAddressRequest();
            inValue.AddressLine = AddressLine;
            inValue.City = City;
            inValue.StateOrProvince = StateOrProvince;
            inValue.PostalCode = PostalCode;
            inValue.CountryCode = CountryCode;
            inValue.SalesAmount = SalesAmount;
            inValue.SalesDate = SalesDate;
            inValue.ReturnMultiple = ReturnMultiple;
            return ((TaxRatesInKansas.OhioWebService.OHFinderServiceSoap)(this)).GetOHSalesTaxByAddressAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeResponse TaxRatesInKansas.OhioWebService.OHFinderServiceSoap.GetOHSalesTaxByZipCode(TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeRequest request) {
            return base.Channel.GetOHSalesTaxByZipCode(request);
        }
        
        public TaxRatesInKansas.OhioWebService.ZipCodeReturn GetOHSalesTaxByZipCode(string PostalCode, double SalesAmount, System.DateTime SalesDate, bool ReturnMultiple) {
            TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeRequest inValue = new TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeRequest();
            inValue.PostalCode = PostalCode;
            inValue.SalesAmount = SalesAmount;
            inValue.SalesDate = SalesDate;
            inValue.ReturnMultiple = ReturnMultiple;
            TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeResponse retVal = ((TaxRatesInKansas.OhioWebService.OHFinderServiceSoap)(this)).GetOHSalesTaxByZipCode(inValue);
            return retVal.GetOHSalesTaxByZipCodeResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeResponse> TaxRatesInKansas.OhioWebService.OHFinderServiceSoap.GetOHSalesTaxByZipCodeAsync(TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeRequest request) {
            return base.Channel.GetOHSalesTaxByZipCodeAsync(request);
        }
        
        public System.Threading.Tasks.Task<TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeResponse> GetOHSalesTaxByZipCodeAsync(string PostalCode, double SalesAmount, System.DateTime SalesDate, bool ReturnMultiple) {
            TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeRequest inValue = new TaxRatesInKansas.OhioWebService.GetOHSalesTaxByZipCodeRequest();
            inValue.PostalCode = PostalCode;
            inValue.SalesAmount = SalesAmount;
            inValue.SalesDate = SalesDate;
            inValue.ReturnMultiple = ReturnMultiple;
            return ((TaxRatesInKansas.OhioWebService.OHFinderServiceSoap)(this)).GetOHSalesTaxByZipCodeAsync(inValue);
        }
    }
}
