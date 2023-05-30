# Semp.V2.CSharp.Api.TelemetryProfileApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateMsgVpnTelemetryProfile**](TelemetryProfileApi.md#createmsgvpntelemetryprofile) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles | Create a Telemetry Profile object.
[**CreateMsgVpnTelemetryProfileReceiverAclConnectException**](TelemetryProfileApi.md#createmsgvpntelemetryprofilereceiveraclconnectexception) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions | Create a Receiver ACL Connect Exception object.
[**CreateMsgVpnTelemetryProfileTraceFilter**](TelemetryProfileApi.md#createmsgvpntelemetryprofiletracefilter) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters | Create a Trace Filter object.
[**CreateMsgVpnTelemetryProfileTraceFilterSubscription**](TelemetryProfileApi.md#createmsgvpntelemetryprofiletracefiltersubscription) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions | Create a Telemetry Trace Filter Subscription object.
[**DeleteMsgVpnTelemetryProfile**](TelemetryProfileApi.md#deletemsgvpntelemetryprofile) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Delete a Telemetry Profile object.
[**DeleteMsgVpnTelemetryProfileReceiverAclConnectException**](TelemetryProfileApi.md#deletemsgvpntelemetryprofilereceiveraclconnectexception) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions/{receiverAclConnectExceptionAddress} | Delete a Receiver ACL Connect Exception object.
[**DeleteMsgVpnTelemetryProfileTraceFilter**](TelemetryProfileApi.md#deletemsgvpntelemetryprofiletracefilter) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Delete a Trace Filter object.
[**DeleteMsgVpnTelemetryProfileTraceFilterSubscription**](TelemetryProfileApi.md#deletemsgvpntelemetryprofiletracefiltersubscription) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions/{subscription},{subscriptionSyntax} | Delete a Telemetry Trace Filter Subscription object.
[**GetMsgVpnTelemetryProfile**](TelemetryProfileApi.md#getmsgvpntelemetryprofile) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Get a Telemetry Profile object.
[**GetMsgVpnTelemetryProfileReceiverAclConnectException**](TelemetryProfileApi.md#getmsgvpntelemetryprofilereceiveraclconnectexception) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions/{receiverAclConnectExceptionAddress} | Get a Receiver ACL Connect Exception object.
[**GetMsgVpnTelemetryProfileReceiverAclConnectExceptions**](TelemetryProfileApi.md#getmsgvpntelemetryprofilereceiveraclconnectexceptions) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions | Get a list of Receiver ACL Connect Exception objects.
[**GetMsgVpnTelemetryProfileTraceFilter**](TelemetryProfileApi.md#getmsgvpntelemetryprofiletracefilter) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Get a Trace Filter object.
[**GetMsgVpnTelemetryProfileTraceFilterSubscription**](TelemetryProfileApi.md#getmsgvpntelemetryprofiletracefiltersubscription) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions/{subscription},{subscriptionSyntax} | Get a Telemetry Trace Filter Subscription object.
[**GetMsgVpnTelemetryProfileTraceFilterSubscriptions**](TelemetryProfileApi.md#getmsgvpntelemetryprofiletracefiltersubscriptions) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions | Get a list of Telemetry Trace Filter Subscription objects.
[**GetMsgVpnTelemetryProfileTraceFilters**](TelemetryProfileApi.md#getmsgvpntelemetryprofiletracefilters) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters | Get a list of Trace Filter objects.
[**GetMsgVpnTelemetryProfiles**](TelemetryProfileApi.md#getmsgvpntelemetryprofiles) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles | Get a list of Telemetry Profile objects.
[**ReplaceMsgVpnTelemetryProfile**](TelemetryProfileApi.md#replacemsgvpntelemetryprofile) | **PUT** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Replace a Telemetry Profile object.
[**ReplaceMsgVpnTelemetryProfileTraceFilter**](TelemetryProfileApi.md#replacemsgvpntelemetryprofiletracefilter) | **PUT** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Replace a Trace Filter object.
[**UpdateMsgVpnTelemetryProfile**](TelemetryProfileApi.md#updatemsgvpntelemetryprofile) | **PATCH** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Update a Telemetry Profile object.
[**UpdateMsgVpnTelemetryProfileTraceFilter**](TelemetryProfileApi.md#updatemsgvpntelemetryprofiletracefilter) | **PATCH** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Update a Trace Filter object.

<a name="createmsgvpntelemetryprofile"></a>
# **CreateMsgVpnTelemetryProfile**
> MsgVpnTelemetryProfileResponse CreateMsgVpnTelemetryProfile (MsgVpnTelemetryProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null)

Create a Telemetry Profile object.

Create a Telemetry Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnTelemetryProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var body = new MsgVpnTelemetryProfile(); // MsgVpnTelemetryProfile | The Telemetry Profile object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Telemetry Profile object.
                MsgVpnTelemetryProfileResponse result = apiInstance.CreateMsgVpnTelemetryProfile(body, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.CreateMsgVpnTelemetryProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTelemetryProfile**](MsgVpnTelemetryProfile.md)| The Telemetry Profile object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileResponse**](MsgVpnTelemetryProfileResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpntelemetryprofilereceiveraclconnectexception"></a>
# **CreateMsgVpnTelemetryProfileReceiverAclConnectException**
> MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse CreateMsgVpnTelemetryProfileReceiverAclConnectException (MsgVpnTelemetryProfileReceiverAclConnectException body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)

Create a Receiver ACL Connect Exception object.

Create a Receiver ACL Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| receiverAclConnectExceptionAddress|x|x|||| telemetryProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnTelemetryProfileReceiverAclConnectExceptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var body = new MsgVpnTelemetryProfileReceiverAclConnectException(); // MsgVpnTelemetryProfileReceiverAclConnectException | The Receiver ACL Connect Exception object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Receiver ACL Connect Exception object.
                MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse result = apiInstance.CreateMsgVpnTelemetryProfileReceiverAclConnectException(body, msgVpnName, telemetryProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.CreateMsgVpnTelemetryProfileReceiverAclConnectException: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTelemetryProfileReceiverAclConnectException**](MsgVpnTelemetryProfileReceiverAclConnectException.md)| The Receiver ACL Connect Exception object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse**](MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpntelemetryprofiletracefilter"></a>
# **CreateMsgVpnTelemetryProfileTraceFilter**
> MsgVpnTelemetryProfileTraceFilterResponse CreateMsgVpnTelemetryProfileTraceFilter (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)

Create a Trace Filter object.

Create a Trace Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter's subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x||x||| traceFilterName|x|x||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnTelemetryProfileTraceFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var body = new MsgVpnTelemetryProfileTraceFilter(); // MsgVpnTelemetryProfileTraceFilter | The Trace Filter object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Trace Filter object.
                MsgVpnTelemetryProfileTraceFilterResponse result = apiInstance.CreateMsgVpnTelemetryProfileTraceFilter(body, msgVpnName, telemetryProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.CreateMsgVpnTelemetryProfileTraceFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTelemetryProfileTraceFilter**](MsgVpnTelemetryProfileTraceFilter.md)| The Trace Filter object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileTraceFilterResponse**](MsgVpnTelemetryProfileTraceFilterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpntelemetryprofiletracefiltersubscription"></a>
# **CreateMsgVpnTelemetryProfileTraceFilterSubscription**
> MsgVpnTelemetryProfileTraceFilterSubscriptionResponse CreateMsgVpnTelemetryProfileTraceFilterSubscription (MsgVpnTelemetryProfileTraceFilterSubscription body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)

Create a Telemetry Trace Filter Subscription object.

Create a Telemetry Trace Filter Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| subscription|x|x|||| subscriptionSyntax|x|x|||| telemetryProfileName|x||x||| traceFilterName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnTelemetryProfileTraceFilterSubscriptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var body = new MsgVpnTelemetryProfileTraceFilterSubscription(); // MsgVpnTelemetryProfileTraceFilterSubscription | The Telemetry Trace Filter Subscription object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var traceFilterName = traceFilterName_example;  // string | A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \"appNameDebug\".
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Telemetry Trace Filter Subscription object.
                MsgVpnTelemetryProfileTraceFilterSubscriptionResponse result = apiInstance.CreateMsgVpnTelemetryProfileTraceFilterSubscription(body, msgVpnName, telemetryProfileName, traceFilterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.CreateMsgVpnTelemetryProfileTraceFilterSubscription: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTelemetryProfileTraceFilterSubscription**](MsgVpnTelemetryProfileTraceFilterSubscription.md)| The Telemetry Trace Filter Subscription object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **traceFilterName** | **string**| A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileTraceFilterSubscriptionResponse**](MsgVpnTelemetryProfileTraceFilterSubscriptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpntelemetryprofile"></a>
# **DeleteMsgVpnTelemetryProfile**
> SempMetaOnlyResponse DeleteMsgVpnTelemetryProfile (string msgVpnName, string telemetryProfileName)

Delete a Telemetry Profile object.

Delete a Telemetry Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnTelemetryProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.

            try
            {
                // Delete a Telemetry Profile object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnTelemetryProfile(msgVpnName, telemetryProfileName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.DeleteMsgVpnTelemetryProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpntelemetryprofilereceiveraclconnectexception"></a>
# **DeleteMsgVpnTelemetryProfileReceiverAclConnectException**
> SempMetaOnlyResponse DeleteMsgVpnTelemetryProfileReceiverAclConnectException (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress)

Delete a Receiver ACL Connect Exception object.

Delete a Receiver ACL Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnTelemetryProfileReceiverAclConnectExceptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var receiverAclConnectExceptionAddress = receiverAclConnectExceptionAddress_example;  // string | The IP address/netmask of the receiver connect exception in CIDR form.

            try
            {
                // Delete a Receiver ACL Connect Exception object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnTelemetryProfileReceiverAclConnectException(msgVpnName, telemetryProfileName, receiverAclConnectExceptionAddress);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.DeleteMsgVpnTelemetryProfileReceiverAclConnectException: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **receiverAclConnectExceptionAddress** | **string**| The IP address/netmask of the receiver connect exception in CIDR form. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpntelemetryprofiletracefilter"></a>
# **DeleteMsgVpnTelemetryProfileTraceFilter**
> SempMetaOnlyResponse DeleteMsgVpnTelemetryProfileTraceFilter (string msgVpnName, string telemetryProfileName, string traceFilterName)

Delete a Trace Filter object.

Delete a Trace Filter object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter's subscription, the message will be traced as it passes through the broker.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnTelemetryProfileTraceFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var traceFilterName = traceFilterName_example;  // string | A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \"appNameDebug\".

            try
            {
                // Delete a Trace Filter object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnTelemetryProfileTraceFilter(msgVpnName, telemetryProfileName, traceFilterName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.DeleteMsgVpnTelemetryProfileTraceFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **traceFilterName** | **string**| A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpntelemetryprofiletracefiltersubscription"></a>
# **DeleteMsgVpnTelemetryProfileTraceFilterSubscription**
> SempMetaOnlyResponse DeleteMsgVpnTelemetryProfileTraceFilterSubscription (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax)

Delete a Telemetry Trace Filter Subscription object.

Delete a Telemetry Trace Filter Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnTelemetryProfileTraceFilterSubscriptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var traceFilterName = traceFilterName_example;  // string | A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \"appNameDebug\".
            var subscription = subscription_example;  // string | Messages matching this subscription will follow this filter's configuration.
            var subscriptionSyntax = subscriptionSyntax_example;  // string | The syntax of the trace filter subscription.

            try
            {
                // Delete a Telemetry Trace Filter Subscription object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnTelemetryProfileTraceFilterSubscription(msgVpnName, telemetryProfileName, traceFilterName, subscription, subscriptionSyntax);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.DeleteMsgVpnTelemetryProfileTraceFilterSubscription: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **traceFilterName** | **string**| A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;. | 
 **subscription** | **string**| Messages matching this subscription will follow this filter&#x27;s configuration. | 
 **subscriptionSyntax** | **string**| The syntax of the trace filter subscription. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpntelemetryprofile"></a>
# **GetMsgVpnTelemetryProfile**
> MsgVpnTelemetryProfileResponse GetMsgVpnTelemetryProfile (string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)

Get a Telemetry Profile object.

Get a Telemetry Profile object.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnTelemetryProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Telemetry Profile object.
                MsgVpnTelemetryProfileResponse result = apiInstance.GetMsgVpnTelemetryProfile(msgVpnName, telemetryProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.GetMsgVpnTelemetryProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileResponse**](MsgVpnTelemetryProfileResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpntelemetryprofilereceiveraclconnectexception"></a>
# **GetMsgVpnTelemetryProfileReceiverAclConnectException**
> MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse GetMsgVpnTelemetryProfileReceiverAclConnectException (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress, string opaquePassword = null, List<string> select = null)

Get a Receiver ACL Connect Exception object.

Get a Receiver ACL Connect Exception object.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnTelemetryProfileReceiverAclConnectExceptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var receiverAclConnectExceptionAddress = receiverAclConnectExceptionAddress_example;  // string | The IP address/netmask of the receiver connect exception in CIDR form.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Receiver ACL Connect Exception object.
                MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse result = apiInstance.GetMsgVpnTelemetryProfileReceiverAclConnectException(msgVpnName, telemetryProfileName, receiverAclConnectExceptionAddress, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.GetMsgVpnTelemetryProfileReceiverAclConnectException: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **receiverAclConnectExceptionAddress** | **string**| The IP address/netmask of the receiver connect exception in CIDR form. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse**](MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpntelemetryprofilereceiveraclconnectexceptions"></a>
# **GetMsgVpnTelemetryProfileReceiverAclConnectExceptions**
> MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse GetMsgVpnTelemetryProfileReceiverAclConnectExceptions (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Receiver ACL Connect Exception objects.

Get a list of Receiver ACL Connect Exception objects.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnTelemetryProfileReceiverAclConnectExceptionsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Receiver ACL Connect Exception objects.
                MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse result = apiInstance.GetMsgVpnTelemetryProfileReceiverAclConnectExceptions(msgVpnName, telemetryProfileName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.GetMsgVpnTelemetryProfileReceiverAclConnectExceptions: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse**](MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpntelemetryprofiletracefilter"></a>
# **GetMsgVpnTelemetryProfileTraceFilter**
> MsgVpnTelemetryProfileTraceFilterResponse GetMsgVpnTelemetryProfileTraceFilter (string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)

Get a Trace Filter object.

Get a Trace Filter object.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter's subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnTelemetryProfileTraceFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var traceFilterName = traceFilterName_example;  // string | A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \"appNameDebug\".
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Trace Filter object.
                MsgVpnTelemetryProfileTraceFilterResponse result = apiInstance.GetMsgVpnTelemetryProfileTraceFilter(msgVpnName, telemetryProfileName, traceFilterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.GetMsgVpnTelemetryProfileTraceFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **traceFilterName** | **string**| A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileTraceFilterResponse**](MsgVpnTelemetryProfileTraceFilterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpntelemetryprofiletracefiltersubscription"></a>
# **GetMsgVpnTelemetryProfileTraceFilterSubscription**
> MsgVpnTelemetryProfileTraceFilterSubscriptionResponse GetMsgVpnTelemetryProfileTraceFilterSubscription (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax, string opaquePassword = null, List<string> select = null)

Get a Telemetry Trace Filter Subscription object.

Get a Telemetry Trace Filter Subscription object.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnTelemetryProfileTraceFilterSubscriptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var traceFilterName = traceFilterName_example;  // string | A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \"appNameDebug\".
            var subscription = subscription_example;  // string | Messages matching this subscription will follow this filter's configuration.
            var subscriptionSyntax = subscriptionSyntax_example;  // string | The syntax of the trace filter subscription.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Telemetry Trace Filter Subscription object.
                MsgVpnTelemetryProfileTraceFilterSubscriptionResponse result = apiInstance.GetMsgVpnTelemetryProfileTraceFilterSubscription(msgVpnName, telemetryProfileName, traceFilterName, subscription, subscriptionSyntax, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.GetMsgVpnTelemetryProfileTraceFilterSubscription: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **traceFilterName** | **string**| A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;. | 
 **subscription** | **string**| Messages matching this subscription will follow this filter&#x27;s configuration. | 
 **subscriptionSyntax** | **string**| The syntax of the trace filter subscription. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileTraceFilterSubscriptionResponse**](MsgVpnTelemetryProfileTraceFilterSubscriptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpntelemetryprofiletracefiltersubscriptions"></a>
# **GetMsgVpnTelemetryProfileTraceFilterSubscriptions**
> MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse GetMsgVpnTelemetryProfileTraceFilterSubscriptions (string msgVpnName, string telemetryProfileName, string traceFilterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Telemetry Trace Filter Subscription objects.

Get a list of Telemetry Trace Filter Subscription objects.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnTelemetryProfileTraceFilterSubscriptionsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var traceFilterName = traceFilterName_example;  // string | A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \"appNameDebug\".
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Telemetry Trace Filter Subscription objects.
                MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse result = apiInstance.GetMsgVpnTelemetryProfileTraceFilterSubscriptions(msgVpnName, telemetryProfileName, traceFilterName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.GetMsgVpnTelemetryProfileTraceFilterSubscriptions: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **traceFilterName** | **string**| A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse**](MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpntelemetryprofiletracefilters"></a>
# **GetMsgVpnTelemetryProfileTraceFilters**
> MsgVpnTelemetryProfileTraceFiltersResponse GetMsgVpnTelemetryProfileTraceFilters (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Trace Filter objects.

Get a list of Trace Filter objects.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter's subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnTelemetryProfileTraceFiltersExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Trace Filter objects.
                MsgVpnTelemetryProfileTraceFiltersResponse result = apiInstance.GetMsgVpnTelemetryProfileTraceFilters(msgVpnName, telemetryProfileName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.GetMsgVpnTelemetryProfileTraceFilters: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileTraceFiltersResponse**](MsgVpnTelemetryProfileTraceFiltersResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpntelemetryprofiles"></a>
# **GetMsgVpnTelemetryProfiles**
> MsgVpnTelemetryProfilesResponse GetMsgVpnTelemetryProfiles (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Telemetry Profile objects.

Get a list of Telemetry Profile objects.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnTelemetryProfilesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Telemetry Profile objects.
                MsgVpnTelemetryProfilesResponse result = apiInstance.GetMsgVpnTelemetryProfiles(msgVpnName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.GetMsgVpnTelemetryProfiles: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfilesResponse**](MsgVpnTelemetryProfilesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpntelemetryprofile"></a>
# **ReplaceMsgVpnTelemetryProfile**
> MsgVpnTelemetryProfileResponse ReplaceMsgVpnTelemetryProfile (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)

Replace a Telemetry Profile object.

Replace a Telemetry Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnTelemetryProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var body = new MsgVpnTelemetryProfile(); // MsgVpnTelemetryProfile | The Telemetry Profile object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Telemetry Profile object.
                MsgVpnTelemetryProfileResponse result = apiInstance.ReplaceMsgVpnTelemetryProfile(body, msgVpnName, telemetryProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.ReplaceMsgVpnTelemetryProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTelemetryProfile**](MsgVpnTelemetryProfile.md)| The Telemetry Profile object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileResponse**](MsgVpnTelemetryProfileResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpntelemetryprofiletracefilter"></a>
# **ReplaceMsgVpnTelemetryProfileTraceFilter**
> MsgVpnTelemetryProfileTraceFilterResponse ReplaceMsgVpnTelemetryProfileTraceFilter (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)

Replace a Trace Filter object.

Replace a Trace Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter's subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x||||| traceFilterName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnTelemetryProfileTraceFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var body = new MsgVpnTelemetryProfileTraceFilter(); // MsgVpnTelemetryProfileTraceFilter | The Trace Filter object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var traceFilterName = traceFilterName_example;  // string | A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \"appNameDebug\".
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Trace Filter object.
                MsgVpnTelemetryProfileTraceFilterResponse result = apiInstance.ReplaceMsgVpnTelemetryProfileTraceFilter(body, msgVpnName, telemetryProfileName, traceFilterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.ReplaceMsgVpnTelemetryProfileTraceFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTelemetryProfileTraceFilter**](MsgVpnTelemetryProfileTraceFilter.md)| The Trace Filter object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **traceFilterName** | **string**| A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileTraceFilterResponse**](MsgVpnTelemetryProfileTraceFilterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpntelemetryprofile"></a>
# **UpdateMsgVpnTelemetryProfile**
> MsgVpnTelemetryProfileResponse UpdateMsgVpnTelemetryProfile (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)

Update a Telemetry Profile object.

Update a Telemetry Profile object. Any attribute missing from the request will be left unchanged.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnTelemetryProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var body = new MsgVpnTelemetryProfile(); // MsgVpnTelemetryProfile | The Telemetry Profile object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Telemetry Profile object.
                MsgVpnTelemetryProfileResponse result = apiInstance.UpdateMsgVpnTelemetryProfile(body, msgVpnName, telemetryProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.UpdateMsgVpnTelemetryProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTelemetryProfile**](MsgVpnTelemetryProfile.md)| The Telemetry Profile object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileResponse**](MsgVpnTelemetryProfileResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpntelemetryprofiletracefilter"></a>
# **UpdateMsgVpnTelemetryProfileTraceFilter**
> MsgVpnTelemetryProfileTraceFilterResponse UpdateMsgVpnTelemetryProfileTraceFilter (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)

Update a Trace Filter object.

Update a Trace Filter object. Any attribute missing from the request will be left unchanged.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter's subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x||||| traceFilterName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.31.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnTelemetryProfileTraceFilterExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new TelemetryProfileApi();
            var body = new MsgVpnTelemetryProfileTraceFilter(); // MsgVpnTelemetryProfileTraceFilter | The Trace Filter object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var telemetryProfileName = telemetryProfileName_example;  // string | The name of the Telemetry Profile.
            var traceFilterName = traceFilterName_example;  // string | A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \"appNameDebug\".
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Trace Filter object.
                MsgVpnTelemetryProfileTraceFilterResponse result = apiInstance.UpdateMsgVpnTelemetryProfileTraceFilter(body, msgVpnName, telemetryProfileName, traceFilterName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling TelemetryProfileApi.UpdateMsgVpnTelemetryProfileTraceFilter: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnTelemetryProfileTraceFilter**](MsgVpnTelemetryProfileTraceFilter.md)| The Trace Filter object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **telemetryProfileName** | **string**| The name of the Telemetry Profile. | 
 **traceFilterName** | **string**| A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnTelemetryProfileTraceFilterResponse**](MsgVpnTelemetryProfileTraceFilterResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
