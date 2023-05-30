# Semp.V2.CSharp.Api.AuthenticationOauthProviderApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateMsgVpnAuthenticationOauthProvider**](AuthenticationOauthProviderApi.md#createmsgvpnauthenticationoauthprovider) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProviders | Create an OAuth Provider object.
[**DeleteMsgVpnAuthenticationOauthProvider**](AuthenticationOauthProviderApi.md#deletemsgvpnauthenticationoauthprovider) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Delete an OAuth Provider object.
[**GetMsgVpnAuthenticationOauthProvider**](AuthenticationOauthProviderApi.md#getmsgvpnauthenticationoauthprovider) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Get an OAuth Provider object.
[**GetMsgVpnAuthenticationOauthProviders**](AuthenticationOauthProviderApi.md#getmsgvpnauthenticationoauthproviders) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProviders | Get a list of OAuth Provider objects.
[**ReplaceMsgVpnAuthenticationOauthProvider**](AuthenticationOauthProviderApi.md#replacemsgvpnauthenticationoauthprovider) | **PUT** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Replace an OAuth Provider object.
[**UpdateMsgVpnAuthenticationOauthProvider**](AuthenticationOauthProviderApi.md#updatemsgvpnauthenticationoauthprovider) | **PATCH** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Update an OAuth Provider object.

<a name="createmsgvpnauthenticationoauthprovider"></a>
# **CreateMsgVpnAuthenticationOauthProvider**
> MsgVpnAuthenticationOauthProviderResponse CreateMsgVpnAuthenticationOauthProvider (MsgVpnAuthenticationOauthProvider body, string msgVpnName, string opaquePassword = null, List<string> select = null)

Create an OAuth Provider object.

Create an OAuth Provider object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  OAuth Providers contain information about the issuer of an OAuth token that is needed to validate the token and derive a client username from it.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: audienceClaimName|||||x| audienceClaimSource|||||x| audienceClaimValue|||||x| audienceValidationEnabled|||||x| authorizationGroupClaimName|||||x| authorizationGroupClaimSource|||||x| authorizationGroupEnabled|||||x| disconnectOnTokenExpirationEnabled|||||x| enabled|||||x| jwksRefreshInterval|||||x| jwksUri|||||x| msgVpnName|x||x||x| oauthProviderName|x|x|||x| tokenIgnoreTimeLimitsEnabled|||||x| tokenIntrospectionParameterName|||||x| tokenIntrospectionPassword||||x|x|x tokenIntrospectionTimeout|||||x| tokenIntrospectionUri|||||x| tokenIntrospectionUsername|||||x| usernameClaimName|||||x| usernameClaimSource|||||x| usernameValidateEnabled|||||x|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been deprecated since 2.25. Replaced by authenticationOauthProfiles.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnAuthenticationOauthProviderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProviderApi();
            var body = new MsgVpnAuthenticationOauthProvider(); // MsgVpnAuthenticationOauthProvider | The OAuth Provider object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create an OAuth Provider object.
                MsgVpnAuthenticationOauthProviderResponse result = apiInstance.CreateMsgVpnAuthenticationOauthProvider(body, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProviderApi.CreateMsgVpnAuthenticationOauthProvider: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnAuthenticationOauthProvider**](MsgVpnAuthenticationOauthProvider.md)| The OAuth Provider object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProviderResponse**](MsgVpnAuthenticationOauthProviderResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnauthenticationoauthprovider"></a>
# **DeleteMsgVpnAuthenticationOauthProvider**
> SempMetaOnlyResponse DeleteMsgVpnAuthenticationOauthProvider (string msgVpnName, string oauthProviderName)

Delete an OAuth Provider object.

Delete an OAuth Provider object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  OAuth Providers contain information about the issuer of an OAuth token that is needed to validate the token and derive a client username from it.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been deprecated since 2.25. Replaced by authenticationOauthProfiles.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnAuthenticationOauthProviderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProviderApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProviderName = oauthProviderName_example;  // string | The name of the OAuth Provider.

            try
            {
                // Delete an OAuth Provider object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnAuthenticationOauthProvider(msgVpnName, oauthProviderName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProviderApi.DeleteMsgVpnAuthenticationOauthProvider: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProviderName** | **string**| The name of the OAuth Provider. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnauthenticationoauthprovider"></a>
# **GetMsgVpnAuthenticationOauthProvider**
> MsgVpnAuthenticationOauthProviderResponse GetMsgVpnAuthenticationOauthProvider (string msgVpnName, string oauthProviderName, string opaquePassword = null, List<string> select = null)

Get an OAuth Provider object.

Get an OAuth Provider object.  OAuth Providers contain information about the issuer of an OAuth token that is needed to validate the token and derive a client username from it.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: audienceClaimName|||x| audienceClaimSource|||x| audienceClaimValue|||x| audienceValidationEnabled|||x| authorizationGroupClaimName|||x| authorizationGroupClaimSource|||x| authorizationGroupEnabled|||x| disconnectOnTokenExpirationEnabled|||x| enabled|||x| jwksRefreshInterval|||x| jwksUri|||x| msgVpnName|x||x| oauthProviderName|x||x| tokenIgnoreTimeLimitsEnabled|||x| tokenIntrospectionParameterName|||x| tokenIntrospectionPassword||x|x|x tokenIntrospectionTimeout|||x| tokenIntrospectionUri|||x| tokenIntrospectionUsername|||x| usernameClaimName|||x| usernameClaimSource|||x| usernameValidateEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been deprecated since 2.25. Replaced by authenticationOauthProfiles.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnAuthenticationOauthProviderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProviderApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProviderName = oauthProviderName_example;  // string | The name of the OAuth Provider.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get an OAuth Provider object.
                MsgVpnAuthenticationOauthProviderResponse result = apiInstance.GetMsgVpnAuthenticationOauthProvider(msgVpnName, oauthProviderName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProviderApi.GetMsgVpnAuthenticationOauthProvider: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProviderName** | **string**| The name of the OAuth Provider. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProviderResponse**](MsgVpnAuthenticationOauthProviderResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnauthenticationoauthproviders"></a>
# **GetMsgVpnAuthenticationOauthProviders**
> MsgVpnAuthenticationOauthProvidersResponse GetMsgVpnAuthenticationOauthProviders (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of OAuth Provider objects.

Get a list of OAuth Provider objects.  OAuth Providers contain information about the issuer of an OAuth token that is needed to validate the token and derive a client username from it.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: audienceClaimName|||x| audienceClaimSource|||x| audienceClaimValue|||x| audienceValidationEnabled|||x| authorizationGroupClaimName|||x| authorizationGroupClaimSource|||x| authorizationGroupEnabled|||x| disconnectOnTokenExpirationEnabled|||x| enabled|||x| jwksRefreshInterval|||x| jwksUri|||x| msgVpnName|x||x| oauthProviderName|x||x| tokenIgnoreTimeLimitsEnabled|||x| tokenIntrospectionParameterName|||x| tokenIntrospectionPassword||x|x|x tokenIntrospectionTimeout|||x| tokenIntrospectionUri|||x| tokenIntrospectionUsername|||x| usernameClaimName|||x| usernameClaimSource|||x| usernameValidateEnabled|||x|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been deprecated since 2.25. Replaced by authenticationOauthProfiles.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnAuthenticationOauthProvidersExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProviderApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of OAuth Provider objects.
                MsgVpnAuthenticationOauthProvidersResponse result = apiInstance.GetMsgVpnAuthenticationOauthProviders(msgVpnName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProviderApi.GetMsgVpnAuthenticationOauthProviders: " + e.Message );
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

[**MsgVpnAuthenticationOauthProvidersResponse**](MsgVpnAuthenticationOauthProvidersResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpnauthenticationoauthprovider"></a>
# **ReplaceMsgVpnAuthenticationOauthProvider**
> MsgVpnAuthenticationOauthProviderResponse ReplaceMsgVpnAuthenticationOauthProvider (MsgVpnAuthenticationOauthProvider body, string msgVpnName, string oauthProviderName, string opaquePassword = null, List<string> select = null)

Replace an OAuth Provider object.

Replace an OAuth Provider object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  OAuth Providers contain information about the issuer of an OAuth token that is needed to validate the token and derive a client username from it.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- audienceClaimName|||||||x| audienceClaimSource|||||||x| audienceClaimValue|||||||x| audienceValidationEnabled|||||||x| authorizationGroupClaimName|||||||x| authorizationGroupClaimSource|||||||x| authorizationGroupEnabled|||||||x| disconnectOnTokenExpirationEnabled|||||||x| enabled|||||||x| jwksRefreshInterval|||||||x| jwksUri|||||||x| msgVpnName|x||x||||x| oauthProviderName|x||x||||x| tokenIgnoreTimeLimitsEnabled|||||||x| tokenIntrospectionParameterName|||||||x| tokenIntrospectionPassword||||x|||x|x tokenIntrospectionTimeout|||||||x| tokenIntrospectionUri|||||||x| tokenIntrospectionUsername|||||||x| usernameClaimName|||||||x| usernameClaimSource|||||||x| usernameValidateEnabled|||||||x|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been deprecated since 2.25. Replaced by authenticationOauthProfiles.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnAuthenticationOauthProviderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProviderApi();
            var body = new MsgVpnAuthenticationOauthProvider(); // MsgVpnAuthenticationOauthProvider | The OAuth Provider object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProviderName = oauthProviderName_example;  // string | The name of the OAuth Provider.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace an OAuth Provider object.
                MsgVpnAuthenticationOauthProviderResponse result = apiInstance.ReplaceMsgVpnAuthenticationOauthProvider(body, msgVpnName, oauthProviderName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProviderApi.ReplaceMsgVpnAuthenticationOauthProvider: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnAuthenticationOauthProvider**](MsgVpnAuthenticationOauthProvider.md)| The OAuth Provider object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProviderName** | **string**| The name of the OAuth Provider. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProviderResponse**](MsgVpnAuthenticationOauthProviderResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpnauthenticationoauthprovider"></a>
# **UpdateMsgVpnAuthenticationOauthProvider**
> MsgVpnAuthenticationOauthProviderResponse UpdateMsgVpnAuthenticationOauthProvider (MsgVpnAuthenticationOauthProvider body, string msgVpnName, string oauthProviderName, string opaquePassword = null, List<string> select = null)

Update an OAuth Provider object.

Update an OAuth Provider object. Any attribute missing from the request will be left unchanged.  OAuth Providers contain information about the issuer of an OAuth token that is needed to validate the token and derive a client username from it.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- audienceClaimName||||||x| audienceClaimSource||||||x| audienceClaimValue||||||x| audienceValidationEnabled||||||x| authorizationGroupClaimName||||||x| authorizationGroupClaimSource||||||x| authorizationGroupEnabled||||||x| disconnectOnTokenExpirationEnabled||||||x| enabled||||||x| jwksRefreshInterval||||||x| jwksUri||||||x| msgVpnName|x|x||||x| oauthProviderName|x|x||||x| tokenIgnoreTimeLimitsEnabled||||||x| tokenIntrospectionParameterName||||||x| tokenIntrospectionPassword|||x|||x|x tokenIntrospectionTimeout||||||x| tokenIntrospectionUri||||||x| tokenIntrospectionUsername||||||x| usernameClaimName||||||x| usernameClaimSource||||||x| usernameValidateEnabled||||||x|    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been deprecated since 2.25. Replaced by authenticationOauthProfiles.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnAuthenticationOauthProviderExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProviderApi();
            var body = new MsgVpnAuthenticationOauthProvider(); // MsgVpnAuthenticationOauthProvider | The OAuth Provider object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProviderName = oauthProviderName_example;  // string | The name of the OAuth Provider.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update an OAuth Provider object.
                MsgVpnAuthenticationOauthProviderResponse result = apiInstance.UpdateMsgVpnAuthenticationOauthProvider(body, msgVpnName, oauthProviderName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProviderApi.UpdateMsgVpnAuthenticationOauthProvider: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnAuthenticationOauthProvider**](MsgVpnAuthenticationOauthProvider.md)| The OAuth Provider object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProviderName** | **string**| The name of the OAuth Provider. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProviderResponse**](MsgVpnAuthenticationOauthProviderResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
