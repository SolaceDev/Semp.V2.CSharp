# Semp.V2.CSharp.Api.RestDeliveryPointApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateMsgVpnRestDeliveryPoint**](RestDeliveryPointApi.md#createmsgvpnrestdeliverypoint) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints | Create a REST Delivery Point object.
[**CreateMsgVpnRestDeliveryPointQueueBinding**](RestDeliveryPointApi.md#createmsgvpnrestdeliverypointqueuebinding) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings | Create a Queue Binding object.
[**CreateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](RestDeliveryPointApi.md#createmsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders | Create a Protected Request Header object.
[**CreateMsgVpnRestDeliveryPointQueueBindingRequestHeader**](RestDeliveryPointApi.md#createmsgvpnrestdeliverypointqueuebindingrequestheader) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders | Create a Request Header object.
[**CreateMsgVpnRestDeliveryPointRestConsumer**](RestDeliveryPointApi.md#createmsgvpnrestdeliverypointrestconsumer) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers | Create a REST Consumer object.
[**CreateMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**](RestDeliveryPointApi.md#createmsgvpnrestdeliverypointrestconsumeroauthjwtclaim) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims | Create a Claim object.
[**CreateMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**](RestDeliveryPointApi.md#createmsgvpnrestdeliverypointrestconsumertlstrustedcommonname) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames | Create a Trusted Common Name object.
[**DeleteMsgVpnRestDeliveryPoint**](RestDeliveryPointApi.md#deletemsgvpnrestdeliverypoint) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Delete a REST Delivery Point object.
[**DeleteMsgVpnRestDeliveryPointQueueBinding**](RestDeliveryPointApi.md#deletemsgvpnrestdeliverypointqueuebinding) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Delete a Queue Binding object.
[**DeleteMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](RestDeliveryPointApi.md#deletemsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Delete a Protected Request Header object.
[**DeleteMsgVpnRestDeliveryPointQueueBindingRequestHeader**](RestDeliveryPointApi.md#deletemsgvpnrestdeliverypointqueuebindingrequestheader) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Delete a Request Header object.
[**DeleteMsgVpnRestDeliveryPointRestConsumer**](RestDeliveryPointApi.md#deletemsgvpnrestdeliverypointrestconsumer) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Delete a REST Consumer object.
[**DeleteMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**](RestDeliveryPointApi.md#deletemsgvpnrestdeliverypointrestconsumeroauthjwtclaim) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims/{oauthJwtClaimName} | Delete a Claim object.
[**DeleteMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**](RestDeliveryPointApi.md#deletemsgvpnrestdeliverypointrestconsumertlstrustedcommonname) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Delete a Trusted Common Name object.
[**GetMsgVpnRestDeliveryPoint**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypoint) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Get a REST Delivery Point object.
[**GetMsgVpnRestDeliveryPointQueueBinding**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypointqueuebinding) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Get a Queue Binding object.
[**GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Get a Protected Request Header object.
[**GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaders**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypointqueuebindingprotectedrequestheaders) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders | Get a list of Protected Request Header objects.
[**GetMsgVpnRestDeliveryPointQueueBindingRequestHeader**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypointqueuebindingrequestheader) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Get a Request Header object.
[**GetMsgVpnRestDeliveryPointQueueBindingRequestHeaders**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypointqueuebindingrequestheaders) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders | Get a list of Request Header objects.
[**GetMsgVpnRestDeliveryPointQueueBindings**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypointqueuebindings) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings | Get a list of Queue Binding objects.
[**GetMsgVpnRestDeliveryPointRestConsumer**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypointrestconsumer) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Get a REST Consumer object.
[**GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypointrestconsumeroauthjwtclaim) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims/{oauthJwtClaimName} | Get a Claim object.
[**GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaims**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypointrestconsumeroauthjwtclaims) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims | Get a list of Claim objects.
[**GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypointrestconsumertlstrustedcommonname) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Get a Trusted Common Name object.
[**GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNames**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypointrestconsumertlstrustedcommonnames) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames | Get a list of Trusted Common Name objects.
[**GetMsgVpnRestDeliveryPointRestConsumers**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypointrestconsumers) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers | Get a list of REST Consumer objects.
[**GetMsgVpnRestDeliveryPoints**](RestDeliveryPointApi.md#getmsgvpnrestdeliverypoints) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints | Get a list of REST Delivery Point objects.
[**ReplaceMsgVpnRestDeliveryPoint**](RestDeliveryPointApi.md#replacemsgvpnrestdeliverypoint) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Replace a REST Delivery Point object.
[**ReplaceMsgVpnRestDeliveryPointQueueBinding**](RestDeliveryPointApi.md#replacemsgvpnrestdeliverypointqueuebinding) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Replace a Queue Binding object.
[**ReplaceMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](RestDeliveryPointApi.md#replacemsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Replace a Protected Request Header object.
[**ReplaceMsgVpnRestDeliveryPointQueueBindingRequestHeader**](RestDeliveryPointApi.md#replacemsgvpnrestdeliverypointqueuebindingrequestheader) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Replace a Request Header object.
[**ReplaceMsgVpnRestDeliveryPointRestConsumer**](RestDeliveryPointApi.md#replacemsgvpnrestdeliverypointrestconsumer) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Replace a REST Consumer object.
[**UpdateMsgVpnRestDeliveryPoint**](RestDeliveryPointApi.md#updatemsgvpnrestdeliverypoint) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Update a REST Delivery Point object.
[**UpdateMsgVpnRestDeliveryPointQueueBinding**](RestDeliveryPointApi.md#updatemsgvpnrestdeliverypointqueuebinding) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Update a Queue Binding object.
[**UpdateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](RestDeliveryPointApi.md#updatemsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Update a Protected Request Header object.
[**UpdateMsgVpnRestDeliveryPointQueueBindingRequestHeader**](RestDeliveryPointApi.md#updatemsgvpnrestdeliverypointqueuebindingrequestheader) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Update a Request Header object.
[**UpdateMsgVpnRestDeliveryPointRestConsumer**](RestDeliveryPointApi.md#updatemsgvpnrestdeliverypointrestconsumer) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Update a REST Consumer object.

<a name="createmsgvpnrestdeliverypoint"></a>
# **CreateMsgVpnRestDeliveryPoint**
> MsgVpnRestDeliveryPointResponse CreateMsgVpnRestDeliveryPoint (MsgVpnRestDeliveryPoint body, string msgVpnName, string opaquePassword = null, List<string> select = null)

Create a REST Delivery Point object.

Create a REST Delivery Point object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A REST Delivery Point manages delivery of messages from queues to a named list of REST Consumers.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| restDeliveryPointName|x|x||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnRestDeliveryPointExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPoint(); // MsgVpnRestDeliveryPoint | The REST Delivery Point object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a REST Delivery Point object.
                MsgVpnRestDeliveryPointResponse result = apiInstance.CreateMsgVpnRestDeliveryPoint(body, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.CreateMsgVpnRestDeliveryPoint: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPoint**](MsgVpnRestDeliveryPoint.md)| The REST Delivery Point object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointResponse**](MsgVpnRestDeliveryPointResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpnrestdeliverypointqueuebinding"></a>
# **CreateMsgVpnRestDeliveryPointQueueBinding**
> MsgVpnRestDeliveryPointQueueBindingResponse CreateMsgVpnRestDeliveryPointQueueBinding (MsgVpnRestDeliveryPointQueueBinding body, string msgVpnName, string restDeliveryPointName, string opaquePassword = null, List<string> select = null)

Create a Queue Binding object.

Create a Queue Binding object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue Binding for a REST Delivery Point attracts messages to be delivered to REST consumers. If the queue does not exist it can be created subsequently, and once the queue is operational the broker performs the queue binding. Removing the queue binding does not delete the queue itself. Similarly, removing the queue does not remove the queue binding, which fails until the queue is recreated or the queue binding is deleted.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| queueBindingName|x|x|||| restDeliveryPointName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnRestDeliveryPointQueueBindingExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointQueueBinding(); // MsgVpnRestDeliveryPointQueueBinding | The Queue Binding object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Queue Binding object.
                MsgVpnRestDeliveryPointQueueBindingResponse result = apiInstance.CreateMsgVpnRestDeliveryPointQueueBinding(body, msgVpnName, restDeliveryPointName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.CreateMsgVpnRestDeliveryPointQueueBinding: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointQueueBinding**](MsgVpnRestDeliveryPointQueueBinding.md)| The Queue Binding object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingResponse**](MsgVpnRestDeliveryPointQueueBindingResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpnrestdeliverypointqueuebindingprotectedrequestheader"></a>
# **CreateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**
> MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse CreateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader (MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader body, string msgVpnName, string restDeliveryPointName, string queueBindingName, string opaquePassword = null, List<string> select = null)

Create a Protected Request Header object.

Create a Protected Request Header object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A protected request header to be added to the HTTP request. Unlike a non-protected request header, the header value cannot be displayed after it is set.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: headerName|x|x|||| headerValue||||x||x msgVpnName|x||x||| queueBindingName|x||x||| restDeliveryPointName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.30.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader(); // MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader | The Protected Request Header object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Protected Request Header object.
                MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse result = apiInstance.CreateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader(body, msgVpnName, restDeliveryPointName, queueBindingName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.CreateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader.md)| The Protected Request Header object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse**](MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpnrestdeliverypointqueuebindingrequestheader"></a>
# **CreateMsgVpnRestDeliveryPointQueueBindingRequestHeader**
> MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse CreateMsgVpnRestDeliveryPointQueueBindingRequestHeader (MsgVpnRestDeliveryPointQueueBindingRequestHeader body, string msgVpnName, string restDeliveryPointName, string queueBindingName, string opaquePassword = null, List<string> select = null)

Create a Request Header object.

Create a Request Header object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A request header to be added to the HTTP request.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: headerName|x|x|||| msgVpnName|x||x||| queueBindingName|x||x||| restDeliveryPointName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.23.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnRestDeliveryPointQueueBindingRequestHeaderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointQueueBindingRequestHeader(); // MsgVpnRestDeliveryPointQueueBindingRequestHeader | The Request Header object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Request Header object.
                MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse result = apiInstance.CreateMsgVpnRestDeliveryPointQueueBindingRequestHeader(body, msgVpnName, restDeliveryPointName, queueBindingName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.CreateMsgVpnRestDeliveryPointQueueBindingRequestHeader: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointQueueBindingRequestHeader**](MsgVpnRestDeliveryPointQueueBindingRequestHeader.md)| The Request Header object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse**](MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpnrestdeliverypointrestconsumer"></a>
# **CreateMsgVpnRestDeliveryPointRestConsumer**
> MsgVpnRestDeliveryPointRestConsumerResponse CreateMsgVpnRestDeliveryPointRestConsumer (MsgVpnRestDeliveryPointRestConsumer body, string msgVpnName, string restDeliveryPointName, string opaquePassword = null, List<string> select = null)

Create a REST Consumer object.

Create a REST Consumer object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  REST Consumer objects establish HTTP connectivity to REST consumer applications who wish to receive messages from a broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authenticationAwsSecretAccessKey||||x||x authenticationClientCertContent||||x||x authenticationClientCertPassword||||x|| authenticationHttpBasicPassword||||x||x authenticationHttpHeaderValue||||x||x authenticationOauthClientSecret||||x||x authenticationOauthJwtSecretKey||||x||x msgVpnName|x||x||| restConsumerName|x|x|||| restDeliveryPointName|x||x|||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnRestDeliveryPointRestConsumer|authenticationClientCertPassword|authenticationClientCertContent| MsgVpnRestDeliveryPointRestConsumer|authenticationHttpBasicPassword|authenticationHttpBasicUsername| MsgVpnRestDeliveryPointRestConsumer|authenticationHttpBasicUsername|authenticationHttpBasicPassword| MsgVpnRestDeliveryPointRestConsumer|remotePort|tlsEnabled| MsgVpnRestDeliveryPointRestConsumer|tlsEnabled|remotePort|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnRestDeliveryPointRestConsumerExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointRestConsumer(); // MsgVpnRestDeliveryPointRestConsumer | The REST Consumer object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a REST Consumer object.
                MsgVpnRestDeliveryPointRestConsumerResponse result = apiInstance.CreateMsgVpnRestDeliveryPointRestConsumer(body, msgVpnName, restDeliveryPointName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.CreateMsgVpnRestDeliveryPointRestConsumer: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointRestConsumer**](MsgVpnRestDeliveryPointRestConsumer.md)| The REST Consumer object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointRestConsumerResponse**](MsgVpnRestDeliveryPointRestConsumerResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpnrestdeliverypointrestconsumeroauthjwtclaim"></a>
# **CreateMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**
> MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimResponse CreateMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim (MsgVpnRestDeliveryPointRestConsumerOauthJwtClaim body, string msgVpnName, string restDeliveryPointName, string restConsumerName, string opaquePassword = null, List<string> select = null)

Create a Claim object.

Create a Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Claim is added to the JWT sent to the OAuth token request endpoint.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| oauthJwtClaimName|x|x|||| oauthJwtClaimValue||x|||| restConsumerName|x||x||| restDeliveryPointName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.21.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnRestDeliveryPointRestConsumerOauthJwtClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointRestConsumerOauthJwtClaim(); // MsgVpnRestDeliveryPointRestConsumerOauthJwtClaim | The Claim object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var restConsumerName = restConsumerName_example;  // string | The name of the REST Consumer.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Claim object.
                MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimResponse result = apiInstance.CreateMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim(body, msgVpnName, restDeliveryPointName, restConsumerName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.CreateMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**](MsgVpnRestDeliveryPointRestConsumerOauthJwtClaim.md)| The Claim object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **restConsumerName** | **string**| The name of the REST Consumer. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimResponse**](MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpnrestdeliverypointrestconsumertlstrustedcommonname"></a>
# **CreateMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**
> MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameResponse CreateMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName (MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName body, string msgVpnName, string restDeliveryPointName, string restConsumerName, string opaquePassword = null, List<string> select = null)

Create a Trusted Common Name object.

Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the REST Consumer are used by encrypted transports to verify the name in the certificate presented by the remote REST consumer. They must include the common name of the remote REST consumer's server certificate.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||x| restConsumerName|x||x||x| restDeliveryPointName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been deprecated since (will be deprecated in next SEMP version). Common Name validation has been replaced by Server Certificate Name validation.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName(); // MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName | The Trusted Common Name object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var restConsumerName = restConsumerName_example;  // string | The name of the REST Consumer.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Trusted Common Name object.
                MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameResponse result = apiInstance.CreateMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName(body, msgVpnName, restDeliveryPointName, restConsumerName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.CreateMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**](MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName.md)| The Trusted Common Name object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **restConsumerName** | **string**| The name of the REST Consumer. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameResponse**](MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnrestdeliverypoint"></a>
# **DeleteMsgVpnRestDeliveryPoint**
> SempMetaOnlyResponse DeleteMsgVpnRestDeliveryPoint (string msgVpnName, string restDeliveryPointName)

Delete a REST Delivery Point object.

Delete a REST Delivery Point object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A REST Delivery Point manages delivery of messages from queues to a named list of REST Consumers.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnRestDeliveryPointExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.

            try
            {
                // Delete a REST Delivery Point object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnRestDeliveryPoint(msgVpnName, restDeliveryPointName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.DeleteMsgVpnRestDeliveryPoint: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnrestdeliverypointqueuebinding"></a>
# **DeleteMsgVpnRestDeliveryPointQueueBinding**
> SempMetaOnlyResponse DeleteMsgVpnRestDeliveryPointQueueBinding (string msgVpnName, string restDeliveryPointName, string queueBindingName)

Delete a Queue Binding object.

Delete a Queue Binding object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Queue Binding for a REST Delivery Point attracts messages to be delivered to REST consumers. If the queue does not exist it can be created subsequently, and once the queue is operational the broker performs the queue binding. Removing the queue binding does not delete the queue itself. Similarly, removing the queue does not remove the queue binding, which fails until the queue is recreated or the queue binding is deleted.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnRestDeliveryPointQueueBindingExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.

            try
            {
                // Delete a Queue Binding object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnRestDeliveryPointQueueBinding(msgVpnName, restDeliveryPointName, queueBindingName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.DeleteMsgVpnRestDeliveryPointQueueBinding: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnrestdeliverypointqueuebindingprotectedrequestheader"></a>
# **DeleteMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**
> SempMetaOnlyResponse DeleteMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader (string msgVpnName, string restDeliveryPointName, string queueBindingName, string headerName)

Delete a Protected Request Header object.

Delete a Protected Request Header object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A protected request header to be added to the HTTP request. Unlike a non-protected request header, the header value cannot be displayed after it is set.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.30.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var headerName = headerName_example;  // string | The name of the protected HTTP request header.

            try
            {
                // Delete a Protected Request Header object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader(msgVpnName, restDeliveryPointName, queueBindingName, headerName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.DeleteMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **headerName** | **string**| The name of the protected HTTP request header. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnrestdeliverypointqueuebindingrequestheader"></a>
# **DeleteMsgVpnRestDeliveryPointQueueBindingRequestHeader**
> SempMetaOnlyResponse DeleteMsgVpnRestDeliveryPointQueueBindingRequestHeader (string msgVpnName, string restDeliveryPointName, string queueBindingName, string headerName)

Delete a Request Header object.

Delete a Request Header object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A request header to be added to the HTTP request.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.23.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnRestDeliveryPointQueueBindingRequestHeaderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var headerName = headerName_example;  // string | The name of the HTTP request header.

            try
            {
                // Delete a Request Header object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnRestDeliveryPointQueueBindingRequestHeader(msgVpnName, restDeliveryPointName, queueBindingName, headerName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.DeleteMsgVpnRestDeliveryPointQueueBindingRequestHeader: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **headerName** | **string**| The name of the HTTP request header. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnrestdeliverypointrestconsumer"></a>
# **DeleteMsgVpnRestDeliveryPointRestConsumer**
> SempMetaOnlyResponse DeleteMsgVpnRestDeliveryPointRestConsumer (string msgVpnName, string restDeliveryPointName, string restConsumerName)

Delete a REST Consumer object.

Delete a REST Consumer object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  REST Consumer objects establish HTTP connectivity to REST consumer applications who wish to receive messages from a broker.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnRestDeliveryPointRestConsumerExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var restConsumerName = restConsumerName_example;  // string | The name of the REST Consumer.

            try
            {
                // Delete a REST Consumer object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnRestDeliveryPointRestConsumer(msgVpnName, restDeliveryPointName, restConsumerName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.DeleteMsgVpnRestDeliveryPointRestConsumer: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **restConsumerName** | **string**| The name of the REST Consumer. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnrestdeliverypointrestconsumeroauthjwtclaim"></a>
# **DeleteMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**
> SempMetaOnlyResponse DeleteMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim (string msgVpnName, string restDeliveryPointName, string restConsumerName, string oauthJwtClaimName)

Delete a Claim object.

Delete a Claim object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Claim is added to the JWT sent to the OAuth token request endpoint.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.21.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnRestDeliveryPointRestConsumerOauthJwtClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var restConsumerName = restConsumerName_example;  // string | The name of the REST Consumer.
            var oauthJwtClaimName = oauthJwtClaimName_example;  // string | The name of the additional claim. Cannot be \"exp\", \"iat\", or \"jti\".

            try
            {
                // Delete a Claim object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim(msgVpnName, restDeliveryPointName, restConsumerName, oauthJwtClaimName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.DeleteMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **restConsumerName** | **string**| The name of the REST Consumer. | 
 **oauthJwtClaimName** | **string**| The name of the additional claim. Cannot be \&quot;exp\&quot;, \&quot;iat\&quot;, or \&quot;jti\&quot;. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnrestdeliverypointrestconsumertlstrustedcommonname"></a>
# **DeleteMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**
> SempMetaOnlyResponse DeleteMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName (string msgVpnName, string restDeliveryPointName, string restConsumerName, string tlsTrustedCommonName)

Delete a Trusted Common Name object.

Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the REST Consumer are used by encrypted transports to verify the name in the certificate presented by the remote REST consumer. They must include the common name of the remote REST consumer's server certificate.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been deprecated since (will be deprecated in next SEMP version). Common Name validation has been replaced by Server Certificate Name validation.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var restConsumerName = restConsumerName_example;  // string | The name of the REST Consumer.
            var tlsTrustedCommonName = tlsTrustedCommonName_example;  // string | The expected trusted common name of the remote certificate.

            try
            {
                // Delete a Trusted Common Name object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName(msgVpnName, restDeliveryPointName, restConsumerName, tlsTrustedCommonName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.DeleteMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **restConsumerName** | **string**| The name of the REST Consumer. | 
 **tlsTrustedCommonName** | **string**| The expected trusted common name of the remote certificate. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypoint"></a>
# **GetMsgVpnRestDeliveryPoint**
> MsgVpnRestDeliveryPointResponse GetMsgVpnRestDeliveryPoint (string msgVpnName, string restDeliveryPointName, string opaquePassword = null, List<string> select = null)

Get a REST Delivery Point object.

Get a REST Delivery Point object.  A REST Delivery Point manages delivery of messages from queues to a named list of REST Consumers.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| restDeliveryPointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a REST Delivery Point object.
                MsgVpnRestDeliveryPointResponse result = apiInstance.GetMsgVpnRestDeliveryPoint(msgVpnName, restDeliveryPointName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPoint: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointResponse**](MsgVpnRestDeliveryPointResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypointqueuebinding"></a>
# **GetMsgVpnRestDeliveryPointQueueBinding**
> MsgVpnRestDeliveryPointQueueBindingResponse GetMsgVpnRestDeliveryPointQueueBinding (string msgVpnName, string restDeliveryPointName, string queueBindingName, string opaquePassword = null, List<string> select = null)

Get a Queue Binding object.

Get a Queue Binding object.  A Queue Binding for a REST Delivery Point attracts messages to be delivered to REST consumers. If the queue does not exist it can be created subsequently, and once the queue is operational the broker performs the queue binding. Removing the queue binding does not delete the queue itself. Similarly, removing the queue does not remove the queue binding, which fails until the queue is recreated or the queue binding is deleted.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueBindingName|x||| restDeliveryPointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointQueueBindingExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Queue Binding object.
                MsgVpnRestDeliveryPointQueueBindingResponse result = apiInstance.GetMsgVpnRestDeliveryPointQueueBinding(msgVpnName, restDeliveryPointName, queueBindingName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPointQueueBinding: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingResponse**](MsgVpnRestDeliveryPointQueueBindingResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypointqueuebindingprotectedrequestheader"></a>
# **GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**
> MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader (string msgVpnName, string restDeliveryPointName, string queueBindingName, string headerName, string opaquePassword = null, List<string> select = null)

Get a Protected Request Header object.

Get a Protected Request Header object.  A protected request header to be added to the HTTP request. Unlike a non-protected request header, the header value cannot be displayed after it is set.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: headerName|x||| headerValue||x||x msgVpnName|x||| queueBindingName|x||| restDeliveryPointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.30.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var headerName = headerName_example;  // string | The name of the protected HTTP request header.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Protected Request Header object.
                MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse result = apiInstance.GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader(msgVpnName, restDeliveryPointName, queueBindingName, headerName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **headerName** | **string**| The name of the protected HTTP request header. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse**](MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypointqueuebindingprotectedrequestheaders"></a>
# **GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaders**
> MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeadersResponse GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaders (string msgVpnName, string restDeliveryPointName, string queueBindingName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Protected Request Header objects.

Get a list of Protected Request Header objects.  A protected request header to be added to the HTTP request. Unlike a non-protected request header, the header value cannot be displayed after it is set.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: headerName|x||| headerValue||x||x msgVpnName|x||| queueBindingName|x||| restDeliveryPointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.30.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeadersExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Protected Request Header objects.
                MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeadersResponse result = apiInstance.GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaders(msgVpnName, restDeliveryPointName, queueBindingName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaders: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeadersResponse**](MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeadersResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypointqueuebindingrequestheader"></a>
# **GetMsgVpnRestDeliveryPointQueueBindingRequestHeader**
> MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse GetMsgVpnRestDeliveryPointQueueBindingRequestHeader (string msgVpnName, string restDeliveryPointName, string queueBindingName, string headerName, string opaquePassword = null, List<string> select = null)

Get a Request Header object.

Get a Request Header object.  A request header to be added to the HTTP request.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: headerName|x||| msgVpnName|x||| queueBindingName|x||| restDeliveryPointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.23.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointQueueBindingRequestHeaderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var headerName = headerName_example;  // string | The name of the HTTP request header.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Request Header object.
                MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse result = apiInstance.GetMsgVpnRestDeliveryPointQueueBindingRequestHeader(msgVpnName, restDeliveryPointName, queueBindingName, headerName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPointQueueBindingRequestHeader: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **headerName** | **string**| The name of the HTTP request header. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse**](MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypointqueuebindingrequestheaders"></a>
# **GetMsgVpnRestDeliveryPointQueueBindingRequestHeaders**
> MsgVpnRestDeliveryPointQueueBindingRequestHeadersResponse GetMsgVpnRestDeliveryPointQueueBindingRequestHeaders (string msgVpnName, string restDeliveryPointName, string queueBindingName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Request Header objects.

Get a list of Request Header objects.  A request header to be added to the HTTP request.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: headerName|x||| msgVpnName|x||| queueBindingName|x||| restDeliveryPointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.23.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointQueueBindingRequestHeadersExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Request Header objects.
                MsgVpnRestDeliveryPointQueueBindingRequestHeadersResponse result = apiInstance.GetMsgVpnRestDeliveryPointQueueBindingRequestHeaders(msgVpnName, restDeliveryPointName, queueBindingName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPointQueueBindingRequestHeaders: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingRequestHeadersResponse**](MsgVpnRestDeliveryPointQueueBindingRequestHeadersResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypointqueuebindings"></a>
# **GetMsgVpnRestDeliveryPointQueueBindings**
> MsgVpnRestDeliveryPointQueueBindingsResponse GetMsgVpnRestDeliveryPointQueueBindings (string msgVpnName, string restDeliveryPointName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Queue Binding objects.

Get a list of Queue Binding objects.  A Queue Binding for a REST Delivery Point attracts messages to be delivered to REST consumers. If the queue does not exist it can be created subsequently, and once the queue is operational the broker performs the queue binding. Removing the queue binding does not delete the queue itself. Similarly, removing the queue does not remove the queue binding, which fails until the queue is recreated or the queue binding is deleted.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| queueBindingName|x||| restDeliveryPointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointQueueBindingsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Queue Binding objects.
                MsgVpnRestDeliveryPointQueueBindingsResponse result = apiInstance.GetMsgVpnRestDeliveryPointQueueBindings(msgVpnName, restDeliveryPointName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPointQueueBindings: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingsResponse**](MsgVpnRestDeliveryPointQueueBindingsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypointrestconsumer"></a>
# **GetMsgVpnRestDeliveryPointRestConsumer**
> MsgVpnRestDeliveryPointRestConsumerResponse GetMsgVpnRestDeliveryPointRestConsumer (string msgVpnName, string restDeliveryPointName, string restConsumerName, string opaquePassword = null, List<string> select = null)

Get a REST Consumer object.

Get a REST Consumer object.  REST Consumer objects establish HTTP connectivity to REST consumer applications who wish to receive messages from a broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationAwsSecretAccessKey||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| authenticationHttpBasicPassword||x||x authenticationHttpHeaderValue||x||x authenticationOauthClientSecret||x||x authenticationOauthJwtSecretKey||x||x msgVpnName|x||| restConsumerName|x||| restDeliveryPointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointRestConsumerExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var restConsumerName = restConsumerName_example;  // string | The name of the REST Consumer.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a REST Consumer object.
                MsgVpnRestDeliveryPointRestConsumerResponse result = apiInstance.GetMsgVpnRestDeliveryPointRestConsumer(msgVpnName, restDeliveryPointName, restConsumerName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPointRestConsumer: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **restConsumerName** | **string**| The name of the REST Consumer. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointRestConsumerResponse**](MsgVpnRestDeliveryPointRestConsumerResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypointrestconsumeroauthjwtclaim"></a>
# **GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**
> MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimResponse GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim (string msgVpnName, string restDeliveryPointName, string restConsumerName, string oauthJwtClaimName, string opaquePassword = null, List<string> select = null)

Get a Claim object.

Get a Claim object.  A Claim is added to the JWT sent to the OAuth token request endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthJwtClaimName|x||| restConsumerName|x||| restDeliveryPointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.21.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var restConsumerName = restConsumerName_example;  // string | The name of the REST Consumer.
            var oauthJwtClaimName = oauthJwtClaimName_example;  // string | The name of the additional claim. Cannot be \"exp\", \"iat\", or \"jti\".
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Claim object.
                MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimResponse result = apiInstance.GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim(msgVpnName, restDeliveryPointName, restConsumerName, oauthJwtClaimName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **restConsumerName** | **string**| The name of the REST Consumer. | 
 **oauthJwtClaimName** | **string**| The name of the additional claim. Cannot be \&quot;exp\&quot;, \&quot;iat\&quot;, or \&quot;jti\&quot;. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimResponse**](MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypointrestconsumeroauthjwtclaims"></a>
# **GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaims**
> MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimsResponse GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaims (string msgVpnName, string restDeliveryPointName, string restConsumerName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Claim objects.

Get a list of Claim objects.  A Claim is added to the JWT sent to the OAuth token request endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthJwtClaimName|x||| restConsumerName|x||| restDeliveryPointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.21.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaimsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var restConsumerName = restConsumerName_example;  // string | The name of the REST Consumer.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Claim objects.
                MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimsResponse result = apiInstance.GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaims(msgVpnName, restDeliveryPointName, restConsumerName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaims: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **restConsumerName** | **string**| The name of the REST Consumer. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimsResponse**](MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypointrestconsumertlstrustedcommonname"></a>
# **GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**
> MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameResponse GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName (string msgVpnName, string restDeliveryPointName, string restConsumerName, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null)

Get a Trusted Common Name object.

Get a Trusted Common Name object.  The Trusted Common Names for the REST Consumer are used by encrypted transports to verify the name in the certificate presented by the remote REST consumer. They must include the common name of the remote REST consumer's server certificate.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x| restConsumerName|x||x| restDeliveryPointName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been deprecated since (will be deprecated in next SEMP version). Common Name validation has been replaced by Server Certificate Name validation.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var restConsumerName = restConsumerName_example;  // string | The name of the REST Consumer.
            var tlsTrustedCommonName = tlsTrustedCommonName_example;  // string | The expected trusted common name of the remote certificate.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Trusted Common Name object.
                MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameResponse result = apiInstance.GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName(msgVpnName, restDeliveryPointName, restConsumerName, tlsTrustedCommonName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **restConsumerName** | **string**| The name of the REST Consumer. | 
 **tlsTrustedCommonName** | **string**| The expected trusted common name of the remote certificate. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameResponse**](MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypointrestconsumertlstrustedcommonnames"></a>
# **GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNames**
> MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNamesResponse GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNames (string msgVpnName, string restDeliveryPointName, string restConsumerName, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Trusted Common Name objects.

Get a list of Trusted Common Name objects.  The Trusted Common Names for the REST Consumer are used by encrypted transports to verify the name in the certificate presented by the remote REST consumer. They must include the common name of the remote REST consumer's server certificate.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x| restConsumerName|x||x| restDeliveryPointName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been deprecated since (will be deprecated in next SEMP version). Common Name validation has been replaced by Server Certificate Name validation.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNamesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var restConsumerName = restConsumerName_example;  // string | The name of the REST Consumer.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Trusted Common Name objects.
                MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNamesResponse result = apiInstance.GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNames(msgVpnName, restDeliveryPointName, restConsumerName, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNames: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **restConsumerName** | **string**| The name of the REST Consumer. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNamesResponse**](MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNamesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypointrestconsumers"></a>
# **GetMsgVpnRestDeliveryPointRestConsumers**
> MsgVpnRestDeliveryPointRestConsumersResponse GetMsgVpnRestDeliveryPointRestConsumers (string msgVpnName, string restDeliveryPointName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of REST Consumer objects.

Get a list of REST Consumer objects.  REST Consumer objects establish HTTP connectivity to REST consumer applications who wish to receive messages from a broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authenticationAwsSecretAccessKey||x||x authenticationClientCertContent||x||x authenticationClientCertPassword||x|| authenticationHttpBasicPassword||x||x authenticationHttpHeaderValue||x||x authenticationOauthClientSecret||x||x authenticationOauthJwtSecretKey||x||x msgVpnName|x||| restConsumerName|x||| restDeliveryPointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointRestConsumersExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of REST Consumer objects.
                MsgVpnRestDeliveryPointRestConsumersResponse result = apiInstance.GetMsgVpnRestDeliveryPointRestConsumers(msgVpnName, restDeliveryPointName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPointRestConsumers: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointRestConsumersResponse**](MsgVpnRestDeliveryPointRestConsumersResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnrestdeliverypoints"></a>
# **GetMsgVpnRestDeliveryPoints**
> MsgVpnRestDeliveryPointsResponse GetMsgVpnRestDeliveryPoints (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of REST Delivery Point objects.

Get a list of REST Delivery Point objects.  A REST Delivery Point manages delivery of messages from queues to a named list of REST Consumers.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| restDeliveryPointName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnRestDeliveryPointsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of REST Delivery Point objects.
                MsgVpnRestDeliveryPointsResponse result = apiInstance.GetMsgVpnRestDeliveryPoints(msgVpnName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.GetMsgVpnRestDeliveryPoints: " + e.Message );
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

[**MsgVpnRestDeliveryPointsResponse**](MsgVpnRestDeliveryPointsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpnrestdeliverypoint"></a>
# **ReplaceMsgVpnRestDeliveryPoint**
> MsgVpnRestDeliveryPointResponse ReplaceMsgVpnRestDeliveryPoint (MsgVpnRestDeliveryPoint body, string msgVpnName, string restDeliveryPointName, string opaquePassword = null, List<string> select = null)

Replace a REST Delivery Point object.

Replace a REST Delivery Point object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A REST Delivery Point manages delivery of messages from queues to a named list of REST Consumers.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientProfileName||||||x|| msgVpnName|x||x||||| restDeliveryPointName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnRestDeliveryPointExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPoint(); // MsgVpnRestDeliveryPoint | The REST Delivery Point object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a REST Delivery Point object.
                MsgVpnRestDeliveryPointResponse result = apiInstance.ReplaceMsgVpnRestDeliveryPoint(body, msgVpnName, restDeliveryPointName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.ReplaceMsgVpnRestDeliveryPoint: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPoint**](MsgVpnRestDeliveryPoint.md)| The REST Delivery Point object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointResponse**](MsgVpnRestDeliveryPointResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpnrestdeliverypointqueuebinding"></a>
# **ReplaceMsgVpnRestDeliveryPointQueueBinding**
> MsgVpnRestDeliveryPointQueueBindingResponse ReplaceMsgVpnRestDeliveryPointQueueBinding (MsgVpnRestDeliveryPointQueueBinding body, string msgVpnName, string restDeliveryPointName, string queueBindingName, string opaquePassword = null, List<string> select = null)

Replace a Queue Binding object.

Replace a Queue Binding object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Queue Binding for a REST Delivery Point attracts messages to be delivered to REST consumers. If the queue does not exist it can be created subsequently, and once the queue is operational the broker performs the queue binding. Removing the queue binding does not delete the queue itself. Similarly, removing the queue does not remove the queue binding, which fails until the queue is recreated or the queue binding is deleted.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| queueBindingName|x||x||||| restDeliveryPointName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnRestDeliveryPointQueueBindingExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointQueueBinding(); // MsgVpnRestDeliveryPointQueueBinding | The Queue Binding object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Queue Binding object.
                MsgVpnRestDeliveryPointQueueBindingResponse result = apiInstance.ReplaceMsgVpnRestDeliveryPointQueueBinding(body, msgVpnName, restDeliveryPointName, queueBindingName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.ReplaceMsgVpnRestDeliveryPointQueueBinding: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointQueueBinding**](MsgVpnRestDeliveryPointQueueBinding.md)| The Queue Binding object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingResponse**](MsgVpnRestDeliveryPointQueueBindingResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpnrestdeliverypointqueuebindingprotectedrequestheader"></a>
# **ReplaceMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**
> MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse ReplaceMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader (MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader body, string msgVpnName, string restDeliveryPointName, string queueBindingName, string headerName, string opaquePassword = null, List<string> select = null)

Replace a Protected Request Header object.

Replace a Protected Request Header object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A protected request header to be added to the HTTP request. Unlike a non-protected request header, the header value cannot be displayed after it is set.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- headerName|x||x||||| headerValue||||x||||x msgVpnName|x||x||||| queueBindingName|x||x||||| restDeliveryPointName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.30.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader(); // MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader | The Protected Request Header object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var headerName = headerName_example;  // string | The name of the protected HTTP request header.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Protected Request Header object.
                MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse result = apiInstance.ReplaceMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader(body, msgVpnName, restDeliveryPointName, queueBindingName, headerName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.ReplaceMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader.md)| The Protected Request Header object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **headerName** | **string**| The name of the protected HTTP request header. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse**](MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpnrestdeliverypointqueuebindingrequestheader"></a>
# **ReplaceMsgVpnRestDeliveryPointQueueBindingRequestHeader**
> MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse ReplaceMsgVpnRestDeliveryPointQueueBindingRequestHeader (MsgVpnRestDeliveryPointQueueBindingRequestHeader body, string msgVpnName, string restDeliveryPointName, string queueBindingName, string headerName, string opaquePassword = null, List<string> select = null)

Replace a Request Header object.

Replace a Request Header object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A request header to be added to the HTTP request.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- headerName|x||x||||| msgVpnName|x||x||||| queueBindingName|x||x||||| restDeliveryPointName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.23.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnRestDeliveryPointQueueBindingRequestHeaderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointQueueBindingRequestHeader(); // MsgVpnRestDeliveryPointQueueBindingRequestHeader | The Request Header object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var headerName = headerName_example;  // string | The name of the HTTP request header.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Request Header object.
                MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse result = apiInstance.ReplaceMsgVpnRestDeliveryPointQueueBindingRequestHeader(body, msgVpnName, restDeliveryPointName, queueBindingName, headerName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.ReplaceMsgVpnRestDeliveryPointQueueBindingRequestHeader: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointQueueBindingRequestHeader**](MsgVpnRestDeliveryPointQueueBindingRequestHeader.md)| The Request Header object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **headerName** | **string**| The name of the HTTP request header. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse**](MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpnrestdeliverypointrestconsumer"></a>
# **ReplaceMsgVpnRestDeliveryPointRestConsumer**
> MsgVpnRestDeliveryPointRestConsumerResponse ReplaceMsgVpnRestDeliveryPointRestConsumer (MsgVpnRestDeliveryPointRestConsumer body, string msgVpnName, string restDeliveryPointName, string restConsumerName, string opaquePassword = null, List<string> select = null)

Replace a REST Consumer object.

Replace a REST Consumer object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  REST Consumer objects establish HTTP connectivity to REST consumer applications who wish to receive messages from a broker.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationAwsSecretAccessKey||||x||||x authenticationClientCertContent||||x||x||x authenticationClientCertPassword||||x||x|| authenticationHttpBasicPassword||||x||x||x authenticationHttpBasicUsername||||||x|| authenticationHttpHeaderValue||||x||||x authenticationOauthClientId||||||x|| authenticationOauthClientScope||||||x|| authenticationOauthClientSecret||||x||x||x authenticationOauthClientTokenEndpoint||||||x|| authenticationOauthClientTokenExpiryDefault||||||x|| authenticationOauthJwtSecretKey||||x||x||x authenticationOauthJwtTokenEndpoint||||||x|| authenticationOauthJwtTokenExpiryDefault||||||x|| authenticationScheme||||||x|| msgVpnName|x||x||||| outgoingConnectionCount||||||x|| remoteHost||||||x|| remotePort||||||x|| restConsumerName|x||x||||| restDeliveryPointName|x||x||||| tlsCipherSuiteList||||||x|| tlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnRestDeliveryPointRestConsumer|authenticationClientCertPassword|authenticationClientCertContent| MsgVpnRestDeliveryPointRestConsumer|authenticationHttpBasicPassword|authenticationHttpBasicUsername| MsgVpnRestDeliveryPointRestConsumer|authenticationHttpBasicUsername|authenticationHttpBasicPassword| MsgVpnRestDeliveryPointRestConsumer|remotePort|tlsEnabled| MsgVpnRestDeliveryPointRestConsumer|tlsEnabled|remotePort|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnRestDeliveryPointRestConsumerExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointRestConsumer(); // MsgVpnRestDeliveryPointRestConsumer | The REST Consumer object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var restConsumerName = restConsumerName_example;  // string | The name of the REST Consumer.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a REST Consumer object.
                MsgVpnRestDeliveryPointRestConsumerResponse result = apiInstance.ReplaceMsgVpnRestDeliveryPointRestConsumer(body, msgVpnName, restDeliveryPointName, restConsumerName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.ReplaceMsgVpnRestDeliveryPointRestConsumer: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointRestConsumer**](MsgVpnRestDeliveryPointRestConsumer.md)| The REST Consumer object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **restConsumerName** | **string**| The name of the REST Consumer. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointRestConsumerResponse**](MsgVpnRestDeliveryPointRestConsumerResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpnrestdeliverypoint"></a>
# **UpdateMsgVpnRestDeliveryPoint**
> MsgVpnRestDeliveryPointResponse UpdateMsgVpnRestDeliveryPoint (MsgVpnRestDeliveryPoint body, string msgVpnName, string restDeliveryPointName, string opaquePassword = null, List<string> select = null)

Update a REST Delivery Point object.

Update a REST Delivery Point object. Any attribute missing from the request will be left unchanged.  A REST Delivery Point manages delivery of messages from queues to a named list of REST Consumers.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientProfileName|||||x|| msgVpnName|x|x||||| restDeliveryPointName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnRestDeliveryPointExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPoint(); // MsgVpnRestDeliveryPoint | The REST Delivery Point object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a REST Delivery Point object.
                MsgVpnRestDeliveryPointResponse result = apiInstance.UpdateMsgVpnRestDeliveryPoint(body, msgVpnName, restDeliveryPointName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.UpdateMsgVpnRestDeliveryPoint: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPoint**](MsgVpnRestDeliveryPoint.md)| The REST Delivery Point object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointResponse**](MsgVpnRestDeliveryPointResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpnrestdeliverypointqueuebinding"></a>
# **UpdateMsgVpnRestDeliveryPointQueueBinding**
> MsgVpnRestDeliveryPointQueueBindingResponse UpdateMsgVpnRestDeliveryPointQueueBinding (MsgVpnRestDeliveryPointQueueBinding body, string msgVpnName, string restDeliveryPointName, string queueBindingName, string opaquePassword = null, List<string> select = null)

Update a Queue Binding object.

Update a Queue Binding object. Any attribute missing from the request will be left unchanged.  A Queue Binding for a REST Delivery Point attracts messages to be delivered to REST consumers. If the queue does not exist it can be created subsequently, and once the queue is operational the broker performs the queue binding. Removing the queue binding does not delete the queue itself. Similarly, removing the queue does not remove the queue binding, which fails until the queue is recreated or the queue binding is deleted.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| queueBindingName|x|x||||| restDeliveryPointName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnRestDeliveryPointQueueBindingExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointQueueBinding(); // MsgVpnRestDeliveryPointQueueBinding | The Queue Binding object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Queue Binding object.
                MsgVpnRestDeliveryPointQueueBindingResponse result = apiInstance.UpdateMsgVpnRestDeliveryPointQueueBinding(body, msgVpnName, restDeliveryPointName, queueBindingName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.UpdateMsgVpnRestDeliveryPointQueueBinding: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointQueueBinding**](MsgVpnRestDeliveryPointQueueBinding.md)| The Queue Binding object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingResponse**](MsgVpnRestDeliveryPointQueueBindingResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpnrestdeliverypointqueuebindingprotectedrequestheader"></a>
# **UpdateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**
> MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse UpdateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader (MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader body, string msgVpnName, string restDeliveryPointName, string queueBindingName, string headerName, string opaquePassword = null, List<string> select = null)

Update a Protected Request Header object.

Update a Protected Request Header object. Any attribute missing from the request will be left unchanged.  A protected request header to be added to the HTTP request. Unlike a non-protected request header, the header value cannot be displayed after it is set.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- headerName|x|x||||| headerValue|||x||||x msgVpnName|x|x||||| queueBindingName|x|x||||| restDeliveryPointName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.30.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader(); // MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader | The Protected Request Header object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var headerName = headerName_example;  // string | The name of the protected HTTP request header.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Protected Request Header object.
                MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse result = apiInstance.UpdateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader(body, msgVpnName, restDeliveryPointName, queueBindingName, headerName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.UpdateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader.md)| The Protected Request Header object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **headerName** | **string**| The name of the protected HTTP request header. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse**](MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpnrestdeliverypointqueuebindingrequestheader"></a>
# **UpdateMsgVpnRestDeliveryPointQueueBindingRequestHeader**
> MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse UpdateMsgVpnRestDeliveryPointQueueBindingRequestHeader (MsgVpnRestDeliveryPointQueueBindingRequestHeader body, string msgVpnName, string restDeliveryPointName, string queueBindingName, string headerName, string opaquePassword = null, List<string> select = null)

Update a Request Header object.

Update a Request Header object. Any attribute missing from the request will be left unchanged.  A request header to be added to the HTTP request.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- headerName|x|x||||| msgVpnName|x|x||||| queueBindingName|x|x||||| restDeliveryPointName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.23.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnRestDeliveryPointQueueBindingRequestHeaderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointQueueBindingRequestHeader(); // MsgVpnRestDeliveryPointQueueBindingRequestHeader | The Request Header object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var queueBindingName = queueBindingName_example;  // string | The name of a queue in the Message VPN.
            var headerName = headerName_example;  // string | The name of the HTTP request header.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Request Header object.
                MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse result = apiInstance.UpdateMsgVpnRestDeliveryPointQueueBindingRequestHeader(body, msgVpnName, restDeliveryPointName, queueBindingName, headerName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.UpdateMsgVpnRestDeliveryPointQueueBindingRequestHeader: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointQueueBindingRequestHeader**](MsgVpnRestDeliveryPointQueueBindingRequestHeader.md)| The Request Header object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **queueBindingName** | **string**| The name of a queue in the Message VPN. | 
 **headerName** | **string**| The name of the HTTP request header. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse**](MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpnrestdeliverypointrestconsumer"></a>
# **UpdateMsgVpnRestDeliveryPointRestConsumer**
> MsgVpnRestDeliveryPointRestConsumerResponse UpdateMsgVpnRestDeliveryPointRestConsumer (MsgVpnRestDeliveryPointRestConsumer body, string msgVpnName, string restDeliveryPointName, string restConsumerName, string opaquePassword = null, List<string> select = null)

Update a REST Consumer object.

Update a REST Consumer object. Any attribute missing from the request will be left unchanged.  REST Consumer objects establish HTTP connectivity to REST consumer applications who wish to receive messages from a broker.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authenticationAwsSecretAccessKey|||x||||x authenticationClientCertContent|||x||x||x authenticationClientCertPassword|||x||x|| authenticationHttpBasicPassword|||x||x||x authenticationHttpBasicUsername|||||x|| authenticationHttpHeaderValue|||x||||x authenticationOauthClientId|||||x|| authenticationOauthClientScope|||||x|| authenticationOauthClientSecret|||x||x||x authenticationOauthClientTokenEndpoint|||||x|| authenticationOauthClientTokenExpiryDefault|||||x|| authenticationOauthJwtSecretKey|||x||x||x authenticationOauthJwtTokenEndpoint|||||x|| authenticationOauthJwtTokenExpiryDefault|||||x|| authenticationScheme|||||x|| msgVpnName|x|x||||| outgoingConnectionCount|||||x|| remoteHost|||||x|| remotePort|||||x|| restConsumerName|x|x||||| restDeliveryPointName|x|x||||| tlsCipherSuiteList|||||x|| tlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnRestDeliveryPointRestConsumer|authenticationClientCertPassword|authenticationClientCertContent| MsgVpnRestDeliveryPointRestConsumer|authenticationHttpBasicPassword|authenticationHttpBasicUsername| MsgVpnRestDeliveryPointRestConsumer|authenticationHttpBasicUsername|authenticationHttpBasicPassword| MsgVpnRestDeliveryPointRestConsumer|remotePort|tlsEnabled| MsgVpnRestDeliveryPointRestConsumer|tlsEnabled|remotePort|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnRestDeliveryPointRestConsumerExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new RestDeliveryPointApi();
            var body = new MsgVpnRestDeliveryPointRestConsumer(); // MsgVpnRestDeliveryPointRestConsumer | The REST Consumer object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var restDeliveryPointName = restDeliveryPointName_example;  // string | The name of the REST Delivery Point.
            var restConsumerName = restConsumerName_example;  // string | The name of the REST Consumer.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a REST Consumer object.
                MsgVpnRestDeliveryPointRestConsumerResponse result = apiInstance.UpdateMsgVpnRestDeliveryPointRestConsumer(body, msgVpnName, restDeliveryPointName, restConsumerName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling RestDeliveryPointApi.UpdateMsgVpnRestDeliveryPointRestConsumer: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnRestDeliveryPointRestConsumer**](MsgVpnRestDeliveryPointRestConsumer.md)| The REST Consumer object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **restDeliveryPointName** | **string**| The name of the REST Delivery Point. | 
 **restConsumerName** | **string**| The name of the REST Consumer. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnRestDeliveryPointRestConsumerResponse**](MsgVpnRestDeliveryPointRestConsumerResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
