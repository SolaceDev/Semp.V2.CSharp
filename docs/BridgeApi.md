# Semp.V2.CSharp.Api.BridgeApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateMsgVpnBridge**](BridgeApi.md#createmsgvpnbridge) | **POST** /msgVpns/{msgVpnName}/bridges | Create a Bridge object.
[**CreateMsgVpnBridgeRemoteMsgVpn**](BridgeApi.md#createmsgvpnbridgeremotemsgvpn) | **POST** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns | Create a Remote Message VPN object.
[**CreateMsgVpnBridgeRemoteSubscription**](BridgeApi.md#createmsgvpnbridgeremotesubscription) | **POST** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions | Create a Remote Subscription object.
[**CreateMsgVpnBridgeTlsTrustedCommonName**](BridgeApi.md#createmsgvpnbridgetlstrustedcommonname) | **POST** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames | Create a Trusted Common Name object.
[**DeleteMsgVpnBridge**](BridgeApi.md#deletemsgvpnbridge) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Delete a Bridge object.
[**DeleteMsgVpnBridgeRemoteMsgVpn**](BridgeApi.md#deletemsgvpnbridgeremotemsgvpn) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Delete a Remote Message VPN object.
[**DeleteMsgVpnBridgeRemoteSubscription**](BridgeApi.md#deletemsgvpnbridgeremotesubscription) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions/{remoteSubscriptionTopic} | Delete a Remote Subscription object.
[**DeleteMsgVpnBridgeTlsTrustedCommonName**](BridgeApi.md#deletemsgvpnbridgetlstrustedcommonname) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Delete a Trusted Common Name object.
[**GetMsgVpnBridge**](BridgeApi.md#getmsgvpnbridge) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Get a Bridge object.
[**GetMsgVpnBridgeRemoteMsgVpn**](BridgeApi.md#getmsgvpnbridgeremotemsgvpn) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Get a Remote Message VPN object.
[**GetMsgVpnBridgeRemoteMsgVpns**](BridgeApi.md#getmsgvpnbridgeremotemsgvpns) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns | Get a list of Remote Message VPN objects.
[**GetMsgVpnBridgeRemoteSubscription**](BridgeApi.md#getmsgvpnbridgeremotesubscription) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions/{remoteSubscriptionTopic} | Get a Remote Subscription object.
[**GetMsgVpnBridgeRemoteSubscriptions**](BridgeApi.md#getmsgvpnbridgeremotesubscriptions) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions | Get a list of Remote Subscription objects.
[**GetMsgVpnBridgeTlsTrustedCommonName**](BridgeApi.md#getmsgvpnbridgetlstrustedcommonname) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Get a Trusted Common Name object.
[**GetMsgVpnBridgeTlsTrustedCommonNames**](BridgeApi.md#getmsgvpnbridgetlstrustedcommonnames) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames | Get a list of Trusted Common Name objects.
[**GetMsgVpnBridges**](BridgeApi.md#getmsgvpnbridges) | **GET** /msgVpns/{msgVpnName}/bridges | Get a list of Bridge objects.
[**ReplaceMsgVpnBridge**](BridgeApi.md#replacemsgvpnbridge) | **PUT** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Replace a Bridge object.
[**ReplaceMsgVpnBridgeRemoteMsgVpn**](BridgeApi.md#replacemsgvpnbridgeremotemsgvpn) | **PUT** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Replace a Remote Message VPN object.
[**UpdateMsgVpnBridge**](BridgeApi.md#updatemsgvpnbridge) | **PATCH** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Update a Bridge object.
[**UpdateMsgVpnBridgeRemoteMsgVpn**](BridgeApi.md#updatemsgvpnbridgeremotemsgvpn) | **PATCH** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Update a Remote Message VPN object.

<a name="createmsgvpnbridge"></a>
# **CreateMsgVpnBridge**
> MsgVpnBridgeResponse CreateMsgVpnBridge (MsgVpnBridge body, string msgVpnName, string opaquePassword = null, List<string> select = null)

Create a Bridge object.

Create a Bridge object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x|x|||| bridgeVirtualRouter|x|x|||| msgVpnName|x||x||| remoteAuthenticationBasicPassword||||x||x remoteAuthenticationClientCertContent||||x||x remoteAuthenticationClientCertPassword||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnBridgeExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var body = new MsgVpnBridge(); // MsgVpnBridge | The Bridge object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Bridge object.
                MsgVpnBridgeResponse result = apiInstance.CreateMsgVpnBridge(body, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.CreateMsgVpnBridge: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnBridge**](MsgVpnBridge.md)| The Bridge object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeResponse**](MsgVpnBridgeResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpnbridgeremotemsgvpn"></a>
# **CreateMsgVpnBridgeRemoteMsgVpn**
> MsgVpnBridgeRemoteMsgVpnResponse CreateMsgVpnBridgeRemoteMsgVpn (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)

Create a Remote Message VPN object.

Create a Remote Message VPN object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| msgVpnName|x||x||| password||||x||x remoteMsgVpnInterface|x||||| remoteMsgVpnLocation|x|x|||| remoteMsgVpnName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnBridgeRemoteMsgVpnExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var body = new MsgVpnBridgeRemoteMsgVpn(); // MsgVpnBridgeRemoteMsgVpn | The Remote Message VPN object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Remote Message VPN object.
                MsgVpnBridgeRemoteMsgVpnResponse result = apiInstance.CreateMsgVpnBridgeRemoteMsgVpn(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.CreateMsgVpnBridgeRemoteMsgVpn: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnBridgeRemoteMsgVpn**](MsgVpnBridgeRemoteMsgVpn.md)| The Remote Message VPN object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeRemoteMsgVpnResponse**](MsgVpnBridgeRemoteMsgVpnResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpnbridgeremotesubscription"></a>
# **CreateMsgVpnBridgeRemoteSubscription**
> MsgVpnBridgeRemoteSubscriptionResponse CreateMsgVpnBridgeRemoteSubscription (MsgVpnBridgeRemoteSubscription body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)

Create a Remote Subscription object.

Create a Remote Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||| bridgeVirtualRouter|x||x||| deliverAlwaysEnabled||x|||| msgVpnName|x||x||| remoteSubscriptionTopic|x|x||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnBridgeRemoteSubscriptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var body = new MsgVpnBridgeRemoteSubscription(); // MsgVpnBridgeRemoteSubscription | The Remote Subscription object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Remote Subscription object.
                MsgVpnBridgeRemoteSubscriptionResponse result = apiInstance.CreateMsgVpnBridgeRemoteSubscription(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.CreateMsgVpnBridgeRemoteSubscription: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnBridgeRemoteSubscription**](MsgVpnBridgeRemoteSubscription.md)| The Remote Subscription object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeRemoteSubscriptionResponse**](MsgVpnBridgeRemoteSubscriptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpnbridgetlstrustedcommonname"></a>
# **CreateMsgVpnBridgeTlsTrustedCommonName**
> MsgVpnBridgeTlsTrustedCommonNameResponse CreateMsgVpnBridgeTlsTrustedCommonName (MsgVpnBridgeTlsTrustedCommonName body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)

Create a Trusted Common Name object.

Create a Trusted Common Name object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node's server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: bridgeName|x||x||x| bridgeVirtualRouter|x||x||x| msgVpnName|x||x||x| tlsTrustedCommonName|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnBridgeTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var body = new MsgVpnBridgeTlsTrustedCommonName(); // MsgVpnBridgeTlsTrustedCommonName | The Trusted Common Name object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Trusted Common Name object.
                MsgVpnBridgeTlsTrustedCommonNameResponse result = apiInstance.CreateMsgVpnBridgeTlsTrustedCommonName(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.CreateMsgVpnBridgeTlsTrustedCommonName: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnBridgeTlsTrustedCommonName**](MsgVpnBridgeTlsTrustedCommonName.md)| The Trusted Common Name object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeTlsTrustedCommonNameResponse**](MsgVpnBridgeTlsTrustedCommonNameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnbridge"></a>
# **DeleteMsgVpnBridge**
> SempMetaOnlyResponse DeleteMsgVpnBridge (string msgVpnName, string bridgeName, string bridgeVirtualRouter)

Delete a Bridge object.

Delete a Bridge object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnBridgeExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.

            try
            {
                // Delete a Bridge object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnBridge(msgVpnName, bridgeName, bridgeVirtualRouter);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.DeleteMsgVpnBridge: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnbridgeremotemsgvpn"></a>
# **DeleteMsgVpnBridgeRemoteMsgVpn**
> SempMetaOnlyResponse DeleteMsgVpnBridgeRemoteMsgVpn (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface)

Delete a Remote Message VPN object.

Delete a Remote Message VPN object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Remote Message VPN is the Message VPN that the Bridge connects to.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnBridgeRemoteMsgVpnExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var remoteMsgVpnName = remoteMsgVpnName_example;  // string | The name of the remote Message VPN.
            var remoteMsgVpnLocation = remoteMsgVpnLocation_example;  // string | The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \"v:\").
            var remoteMsgVpnInterface = remoteMsgVpnInterface_example;  // string | The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, `remoteMsgVpnLocation` must not be a virtual router name.

            try
            {
                // Delete a Remote Message VPN object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnBridgeRemoteMsgVpn(msgVpnName, bridgeName, bridgeVirtualRouter, remoteMsgVpnName, remoteMsgVpnLocation, remoteMsgVpnInterface);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.DeleteMsgVpnBridgeRemoteMsgVpn: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **remoteMsgVpnName** | **string**| The name of the remote Message VPN. | 
 **remoteMsgVpnLocation** | **string**| The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;). | 
 **remoteMsgVpnInterface** | **string**| The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnbridgeremotesubscription"></a>
# **DeleteMsgVpnBridgeRemoteSubscription**
> SempMetaOnlyResponse DeleteMsgVpnBridgeRemoteSubscription (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic)

Delete a Remote Subscription object.

Delete a Remote Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnBridgeRemoteSubscriptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var remoteSubscriptionTopic = remoteSubscriptionTopic_example;  // string | The topic of the Bridge remote subscription.

            try
            {
                // Delete a Remote Subscription object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnBridgeRemoteSubscription(msgVpnName, bridgeName, bridgeVirtualRouter, remoteSubscriptionTopic);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.DeleteMsgVpnBridgeRemoteSubscription: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **remoteSubscriptionTopic** | **string**| The topic of the Bridge remote subscription. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnbridgetlstrustedcommonname"></a>
# **DeleteMsgVpnBridgeTlsTrustedCommonName**
> SempMetaOnlyResponse DeleteMsgVpnBridgeTlsTrustedCommonName (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName)

Delete a Trusted Common Name object.

Delete a Trusted Common Name object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node's server certificate or client certificate, depending upon the initiator of the connection.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnBridgeTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var tlsTrustedCommonName = tlsTrustedCommonName_example;  // string | The expected trusted common name of the remote certificate.

            try
            {
                // Delete a Trusted Common Name object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnBridgeTlsTrustedCommonName(msgVpnName, bridgeName, bridgeVirtualRouter, tlsTrustedCommonName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.DeleteMsgVpnBridgeTlsTrustedCommonName: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **tlsTrustedCommonName** | **string**| The expected trusted common name of the remote certificate. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnbridge"></a>
# **GetMsgVpnBridge**
> MsgVpnBridgeResponse GetMsgVpnBridge (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)

Get a Bridge object.

Get a Bridge object.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnBridgeExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Bridge object.
                MsgVpnBridgeResponse result = apiInstance.GetMsgVpnBridge(msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.GetMsgVpnBridge: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeResponse**](MsgVpnBridgeResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnbridgeremotemsgvpn"></a>
# **GetMsgVpnBridgeRemoteMsgVpn**
> MsgVpnBridgeRemoteMsgVpnResponse GetMsgVpnBridgeRemoteMsgVpn (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)

Get a Remote Message VPN object.

Get a Remote Message VPN object.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnBridgeRemoteMsgVpnExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var remoteMsgVpnName = remoteMsgVpnName_example;  // string | The name of the remote Message VPN.
            var remoteMsgVpnLocation = remoteMsgVpnLocation_example;  // string | The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \"v:\").
            var remoteMsgVpnInterface = remoteMsgVpnInterface_example;  // string | The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, `remoteMsgVpnLocation` must not be a virtual router name.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Remote Message VPN object.
                MsgVpnBridgeRemoteMsgVpnResponse result = apiInstance.GetMsgVpnBridgeRemoteMsgVpn(msgVpnName, bridgeName, bridgeVirtualRouter, remoteMsgVpnName, remoteMsgVpnLocation, remoteMsgVpnInterface, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.GetMsgVpnBridgeRemoteMsgVpn: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **remoteMsgVpnName** | **string**| The name of the remote Message VPN. | 
 **remoteMsgVpnLocation** | **string**| The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;). | 
 **remoteMsgVpnInterface** | **string**| The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeRemoteMsgVpnResponse**](MsgVpnBridgeRemoteMsgVpnResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnbridgeremotemsgvpns"></a>
# **GetMsgVpnBridgeRemoteMsgVpns**
> MsgVpnBridgeRemoteMsgVpnsResponse GetMsgVpnBridgeRemoteMsgVpns (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Remote Message VPN objects.

Get a list of Remote Message VPN objects.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| password||x||x remoteMsgVpnInterface|x||| remoteMsgVpnLocation|x||| remoteMsgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnBridgeRemoteMsgVpnsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Remote Message VPN objects.
                MsgVpnBridgeRemoteMsgVpnsResponse result = apiInstance.GetMsgVpnBridgeRemoteMsgVpns(msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.GetMsgVpnBridgeRemoteMsgVpns: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeRemoteMsgVpnsResponse**](MsgVpnBridgeRemoteMsgVpnsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnbridgeremotesubscription"></a>
# **GetMsgVpnBridgeRemoteSubscription**
> MsgVpnBridgeRemoteSubscriptionResponse GetMsgVpnBridgeRemoteSubscription (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteSubscriptionTopic, string opaquePassword = null, List<string> select = null)

Get a Remote Subscription object.

Get a Remote Subscription object.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnBridgeRemoteSubscriptionExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var remoteSubscriptionTopic = remoteSubscriptionTopic_example;  // string | The topic of the Bridge remote subscription.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Remote Subscription object.
                MsgVpnBridgeRemoteSubscriptionResponse result = apiInstance.GetMsgVpnBridgeRemoteSubscription(msgVpnName, bridgeName, bridgeVirtualRouter, remoteSubscriptionTopic, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.GetMsgVpnBridgeRemoteSubscription: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **remoteSubscriptionTopic** | **string**| The topic of the Bridge remote subscription. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeRemoteSubscriptionResponse**](MsgVpnBridgeRemoteSubscriptionResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnbridgeremotesubscriptions"></a>
# **GetMsgVpnBridgeRemoteSubscriptions**
> MsgVpnBridgeRemoteSubscriptionsResponse GetMsgVpnBridgeRemoteSubscriptions (string msgVpnName, string bridgeName, string bridgeVirtualRouter, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Remote Subscription objects.

Get a list of Remote Subscription objects.  A Remote Subscription is a topic subscription used by the Message VPN Bridge to attract messages from the remote message broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteSubscriptionTopic|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnBridgeRemoteSubscriptionsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Remote Subscription objects.
                MsgVpnBridgeRemoteSubscriptionsResponse result = apiInstance.GetMsgVpnBridgeRemoteSubscriptions(msgVpnName, bridgeName, bridgeVirtualRouter, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.GetMsgVpnBridgeRemoteSubscriptions: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeRemoteSubscriptionsResponse**](MsgVpnBridgeRemoteSubscriptionsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnbridgetlstrustedcommonname"></a>
# **GetMsgVpnBridgeTlsTrustedCommonName**
> MsgVpnBridgeTlsTrustedCommonNameResponse GetMsgVpnBridgeTlsTrustedCommonName (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string tlsTrustedCommonName, string opaquePassword = null, List<string> select = null)

Get a Trusted Common Name object.

Get a Trusted Common Name object.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node's server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnBridgeTlsTrustedCommonNameExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var tlsTrustedCommonName = tlsTrustedCommonName_example;  // string | The expected trusted common name of the remote certificate.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Trusted Common Name object.
                MsgVpnBridgeTlsTrustedCommonNameResponse result = apiInstance.GetMsgVpnBridgeTlsTrustedCommonName(msgVpnName, bridgeName, bridgeVirtualRouter, tlsTrustedCommonName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.GetMsgVpnBridgeTlsTrustedCommonName: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **tlsTrustedCommonName** | **string**| The expected trusted common name of the remote certificate. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeTlsTrustedCommonNameResponse**](MsgVpnBridgeTlsTrustedCommonNameResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnbridgetlstrustedcommonnames"></a>
# **GetMsgVpnBridgeTlsTrustedCommonNames**
> MsgVpnBridgeTlsTrustedCommonNamesResponse GetMsgVpnBridgeTlsTrustedCommonNames (string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Trusted Common Name objects.

Get a list of Trusted Common Name objects.  The Trusted Common Names for the Bridge are used by encrypted transports to verify the name in the certificate presented by the remote node. They must include the common name of the remote node's server certificate or client certificate, depending upon the initiator of the connection.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||x| bridgeVirtualRouter|x||x| msgVpnName|x||x| tlsTrustedCommonName|x||x|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been deprecated since 2.18. Common Name validation has been replaced by Server Certificate Name validation.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnBridgeTlsTrustedCommonNamesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Trusted Common Name objects.
                MsgVpnBridgeTlsTrustedCommonNamesResponse result = apiInstance.GetMsgVpnBridgeTlsTrustedCommonNames(msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.GetMsgVpnBridgeTlsTrustedCommonNames: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeTlsTrustedCommonNamesResponse**](MsgVpnBridgeTlsTrustedCommonNamesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnbridges"></a>
# **GetMsgVpnBridges**
> MsgVpnBridgesResponse GetMsgVpnBridges (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Bridge objects.

Get a list of Bridge objects.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: bridgeName|x||| bridgeVirtualRouter|x||| msgVpnName|x||| remoteAuthenticationBasicPassword||x||x remoteAuthenticationClientCertContent||x||x remoteAuthenticationClientCertPassword||x||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnBridgesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Bridge objects.
                MsgVpnBridgesResponse result = apiInstance.GetMsgVpnBridges(msgVpnName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.GetMsgVpnBridges: " + e.Message );
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

[**MsgVpnBridgesResponse**](MsgVpnBridgesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpnbridge"></a>
# **ReplaceMsgVpnBridge**
> MsgVpnBridgeResponse ReplaceMsgVpnBridge (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)

Replace a Bridge object.

Replace a Bridge object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| maxTtl||||||x|| msgVpnName|x||x||||| remoteAuthenticationBasicClientUsername||||||x|| remoteAuthenticationBasicPassword||||x||x||x remoteAuthenticationClientCertContent||||x||x||x remoteAuthenticationClientCertPassword||||x||x|| remoteAuthenticationScheme||||||x|| remoteDeliverToOnePriority||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnBridgeExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var body = new MsgVpnBridge(); // MsgVpnBridge | The Bridge object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Bridge object.
                MsgVpnBridgeResponse result = apiInstance.ReplaceMsgVpnBridge(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.ReplaceMsgVpnBridge: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnBridge**](MsgVpnBridge.md)| The Bridge object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeResponse**](MsgVpnBridgeResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpnbridgeremotemsgvpn"></a>
# **ReplaceMsgVpnBridgeRemoteMsgVpn**
> MsgVpnBridgeRemoteMsgVpnResponse ReplaceMsgVpnBridgeRemoteMsgVpn (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)

Replace a Remote Message VPN object.

Replace a Remote Message VPN object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x||x||||| bridgeVirtualRouter|x||x||||| clientUsername||||||x|| compressedDataEnabled||||||x|| egressFlowWindowSize||||||x|| msgVpnName|x||x||||| password||||x||x||x remoteMsgVpnInterface|x||x||||| remoteMsgVpnLocation|x||x||||| remoteMsgVpnName|x||x||||| tlsEnabled||||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnBridgeRemoteMsgVpnExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var body = new MsgVpnBridgeRemoteMsgVpn(); // MsgVpnBridgeRemoteMsgVpn | The Remote Message VPN object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var remoteMsgVpnName = remoteMsgVpnName_example;  // string | The name of the remote Message VPN.
            var remoteMsgVpnLocation = remoteMsgVpnLocation_example;  // string | The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \"v:\").
            var remoteMsgVpnInterface = remoteMsgVpnInterface_example;  // string | The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, `remoteMsgVpnLocation` must not be a virtual router name.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace a Remote Message VPN object.
                MsgVpnBridgeRemoteMsgVpnResponse result = apiInstance.ReplaceMsgVpnBridgeRemoteMsgVpn(body, msgVpnName, bridgeName, bridgeVirtualRouter, remoteMsgVpnName, remoteMsgVpnLocation, remoteMsgVpnInterface, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.ReplaceMsgVpnBridgeRemoteMsgVpn: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnBridgeRemoteMsgVpn**](MsgVpnBridgeRemoteMsgVpn.md)| The Remote Message VPN object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **remoteMsgVpnName** | **string**| The name of the remote Message VPN. | 
 **remoteMsgVpnLocation** | **string**| The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;). | 
 **remoteMsgVpnInterface** | **string**| The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeRemoteMsgVpnResponse**](MsgVpnBridgeRemoteMsgVpnResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpnbridge"></a>
# **UpdateMsgVpnBridge**
> MsgVpnBridgeResponse UpdateMsgVpnBridge (MsgVpnBridge body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string opaquePassword = null, List<string> select = null)

Update a Bridge object.

Update a Bridge object. Any attribute missing from the request will be left unchanged.  Bridges can be used to link two Message VPNs so that messages published to one Message VPN that match the topic subscriptions set for the bridge are also delivered to the linked Message VPN.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| maxTtl|||||x|| msgVpnName|x|x||||| remoteAuthenticationBasicClientUsername|||||x|| remoteAuthenticationBasicPassword|||x||x||x remoteAuthenticationClientCertContent|||x||x||x remoteAuthenticationClientCertPassword|||x||x|| remoteAuthenticationScheme|||||x|| remoteDeliverToOnePriority|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridge|remoteAuthenticationBasicClientUsername|remoteAuthenticationBasicPassword| MsgVpnBridge|remoteAuthenticationBasicPassword|remoteAuthenticationBasicClientUsername| MsgVpnBridge|remoteAuthenticationClientCertPassword|remoteAuthenticationClientCertContent|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnBridgeExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var body = new MsgVpnBridge(); // MsgVpnBridge | The Bridge object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Bridge object.
                MsgVpnBridgeResponse result = apiInstance.UpdateMsgVpnBridge(body, msgVpnName, bridgeName, bridgeVirtualRouter, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.UpdateMsgVpnBridge: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnBridge**](MsgVpnBridge.md)| The Bridge object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeResponse**](MsgVpnBridgeResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpnbridgeremotemsgvpn"></a>
# **UpdateMsgVpnBridgeRemoteMsgVpn**
> MsgVpnBridgeRemoteMsgVpnResponse UpdateMsgVpnBridgeRemoteMsgVpn (MsgVpnBridgeRemoteMsgVpn body, string msgVpnName, string bridgeName, string bridgeVirtualRouter, string remoteMsgVpnName, string remoteMsgVpnLocation, string remoteMsgVpnInterface, string opaquePassword = null, List<string> select = null)

Update a Remote Message VPN object.

Update a Remote Message VPN object. Any attribute missing from the request will be left unchanged.  The Remote Message VPN is the Message VPN that the Bridge connects to.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- bridgeName|x|x||||| bridgeVirtualRouter|x|x||||| clientUsername|||||x|| compressedDataEnabled|||||x|| egressFlowWindowSize|||||x|| msgVpnName|x|x||||| password|||x||x||x remoteMsgVpnInterface|x|x||||| remoteMsgVpnLocation|x|x||||| remoteMsgVpnName|x|x||||| tlsEnabled|||||x||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- MsgVpnBridgeRemoteMsgVpn|clientUsername|password| MsgVpnBridgeRemoteMsgVpn|password|clientUsername|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.0.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnBridgeRemoteMsgVpnExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new BridgeApi();
            var body = new MsgVpnBridgeRemoteMsgVpn(); // MsgVpnBridgeRemoteMsgVpn | The Remote Message VPN object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var bridgeName = bridgeName_example;  // string | The name of the Bridge.
            var bridgeVirtualRouter = bridgeVirtualRouter_example;  // string | The virtual router of the Bridge.
            var remoteMsgVpnName = remoteMsgVpnName_example;  // string | The name of the remote Message VPN.
            var remoteMsgVpnLocation = remoteMsgVpnLocation_example;  // string | The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \"v:\").
            var remoteMsgVpnInterface = remoteMsgVpnInterface_example;  // string | The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, `remoteMsgVpnLocation` must not be a virtual router name.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update a Remote Message VPN object.
                MsgVpnBridgeRemoteMsgVpnResponse result = apiInstance.UpdateMsgVpnBridgeRemoteMsgVpn(body, msgVpnName, bridgeName, bridgeVirtualRouter, remoteMsgVpnName, remoteMsgVpnLocation, remoteMsgVpnInterface, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling BridgeApi.UpdateMsgVpnBridgeRemoteMsgVpn: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnBridgeRemoteMsgVpn**](MsgVpnBridgeRemoteMsgVpn.md)| The Remote Message VPN object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **bridgeName** | **string**| The name of the Bridge. | 
 **bridgeVirtualRouter** | **string**| The virtual router of the Bridge. | 
 **remoteMsgVpnName** | **string**| The name of the remote Message VPN. | 
 **remoteMsgVpnLocation** | **string**| The location of the remote Message VPN as either an FQDN with port, IP address with port, or virtual router name (starting with \&quot;v:\&quot;). | 
 **remoteMsgVpnInterface** | **string**| The physical interface on the local Message VPN host for connecting to the remote Message VPN. By default, an interface is chosen automatically (recommended), but if specified, &#x60;remoteMsgVpnLocation&#x60; must not be a virtual router name. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnBridgeRemoteMsgVpnResponse**](MsgVpnBridgeRemoteMsgVpnResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
