# Semp.V2.CSharp.Api.AuthenticationOauthProfileApi

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Method | HTTP request | Description
------------- | ------------- | -------------
[**CreateMsgVpnAuthenticationOauthProfile**](AuthenticationOauthProfileApi.md#createmsgvpnauthenticationoauthprofile) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProfiles | Create an OAuth Profile object.
[**CreateMsgVpnAuthenticationOauthProfileClientRequiredClaim**](AuthenticationOauthProfileApi.md#createmsgvpnauthenticationoauthprofileclientrequiredclaim) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims | Create a Required Claim object.
[**CreateMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**](AuthenticationOauthProfileApi.md#createmsgvpnauthenticationoauthprofileresourceserverrequiredclaim) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Create a Required Claim object.
[**DeleteMsgVpnAuthenticationOauthProfile**](AuthenticationOauthProfileApi.md#deletemsgvpnauthenticationoauthprofile) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Delete an OAuth Profile object.
[**DeleteMsgVpnAuthenticationOauthProfileClientRequiredClaim**](AuthenticationOauthProfileApi.md#deletemsgvpnauthenticationoauthprofileclientrequiredclaim) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Delete a Required Claim object.
[**DeleteMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**](AuthenticationOauthProfileApi.md#deletemsgvpnauthenticationoauthprofileresourceserverrequiredclaim) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Delete a Required Claim object.
[**GetMsgVpnAuthenticationOauthProfile**](AuthenticationOauthProfileApi.md#getmsgvpnauthenticationoauthprofile) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Get an OAuth Profile object.
[**GetMsgVpnAuthenticationOauthProfileClientRequiredClaim**](AuthenticationOauthProfileApi.md#getmsgvpnauthenticationoauthprofileclientrequiredclaim) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Get a Required Claim object.
[**GetMsgVpnAuthenticationOauthProfileClientRequiredClaims**](AuthenticationOauthProfileApi.md#getmsgvpnauthenticationoauthprofileclientrequiredclaims) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims | Get a list of Required Claim objects.
[**GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**](AuthenticationOauthProfileApi.md#getmsgvpnauthenticationoauthprofileresourceserverrequiredclaim) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Get a Required Claim object.
[**GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaims**](AuthenticationOauthProfileApi.md#getmsgvpnauthenticationoauthprofileresourceserverrequiredclaims) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Get a list of Required Claim objects.
[**GetMsgVpnAuthenticationOauthProfiles**](AuthenticationOauthProfileApi.md#getmsgvpnauthenticationoauthprofiles) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles | Get a list of OAuth Profile objects.
[**ReplaceMsgVpnAuthenticationOauthProfile**](AuthenticationOauthProfileApi.md#replacemsgvpnauthenticationoauthprofile) | **PUT** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Replace an OAuth Profile object.
[**UpdateMsgVpnAuthenticationOauthProfile**](AuthenticationOauthProfileApi.md#updatemsgvpnauthenticationoauthprofile) | **PATCH** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Update an OAuth Profile object.

<a name="createmsgvpnauthenticationoauthprofile"></a>
# **CreateMsgVpnAuthenticationOauthProfile**
> MsgVpnAuthenticationOauthProfileResponse CreateMsgVpnAuthenticationOauthProfile (MsgVpnAuthenticationOauthProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null)

Create an OAuth Profile object.

Create an OAuth Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientSecret||||x||x msgVpnName|x||x||| oauthProfileName|x|x||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnAuthenticationOauthProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var body = new MsgVpnAuthenticationOauthProfile(); // MsgVpnAuthenticationOauthProfile | The OAuth Profile object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create an OAuth Profile object.
                MsgVpnAuthenticationOauthProfileResponse result = apiInstance.CreateMsgVpnAuthenticationOauthProfile(body, msgVpnName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.CreateMsgVpnAuthenticationOauthProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnAuthenticationOauthProfile**](MsgVpnAuthenticationOauthProfile.md)| The OAuth Profile object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProfileResponse**](MsgVpnAuthenticationOauthProfileResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpnauthenticationoauthprofileclientrequiredclaim"></a>
# **CreateMsgVpnAuthenticationOauthProfileClientRequiredClaim**
> MsgVpnAuthenticationOauthProfileClientRequiredClaimResponse CreateMsgVpnAuthenticationOauthProfileClientRequiredClaim (MsgVpnAuthenticationOauthProfileClientRequiredClaim body, string msgVpnName, string oauthProfileName, string opaquePassword = null, List<string> select = null)

Create a Required Claim object.

Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Additional claims to be verified in the ID token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x|x|||| clientRequiredClaimValue||x|||| msgVpnName|x||x||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnAuthenticationOauthProfileClientRequiredClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var body = new MsgVpnAuthenticationOauthProfileClientRequiredClaim(); // MsgVpnAuthenticationOauthProfileClientRequiredClaim | The Required Claim object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Required Claim object.
                MsgVpnAuthenticationOauthProfileClientRequiredClaimResponse result = apiInstance.CreateMsgVpnAuthenticationOauthProfileClientRequiredClaim(body, msgVpnName, oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.CreateMsgVpnAuthenticationOauthProfileClientRequiredClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnAuthenticationOauthProfileClientRequiredClaim**](MsgVpnAuthenticationOauthProfileClientRequiredClaim.md)| The Required Claim object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProfileClientRequiredClaimResponse**](MsgVpnAuthenticationOauthProfileClientRequiredClaimResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="createmsgvpnauthenticationoauthprofileresourceserverrequiredclaim"></a>
# **CreateMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**
> MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimResponse CreateMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim (MsgVpnAuthenticationOauthProfileResourceServerRequiredClaim body, string msgVpnName, string oauthProfileName, string opaquePassword = null, List<string> select = null)

Create a Required Claim object.

Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Additional claims to be verified in the access token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| oauthProfileName|x||x||| resourceServerRequiredClaimName|x|x|||| resourceServerRequiredClaimValue||x||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class CreateMsgVpnAuthenticationOauthProfileResourceServerRequiredClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var body = new MsgVpnAuthenticationOauthProfileResourceServerRequiredClaim(); // MsgVpnAuthenticationOauthProfileResourceServerRequiredClaim | The Required Claim object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Create a Required Claim object.
                MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimResponse result = apiInstance.CreateMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim(body, msgVpnName, oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.CreateMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**](MsgVpnAuthenticationOauthProfileResourceServerRequiredClaim.md)| The Required Claim object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimResponse**](MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnauthenticationoauthprofile"></a>
# **DeleteMsgVpnAuthenticationOauthProfile**
> SempMetaOnlyResponse DeleteMsgVpnAuthenticationOauthProfile (string msgVpnName, string oauthProfileName)

Delete an OAuth Profile object.

Delete an OAuth Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnAuthenticationOauthProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.

            try
            {
                // Delete an OAuth Profile object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnAuthenticationOauthProfile(msgVpnName, oauthProfileName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.DeleteMsgVpnAuthenticationOauthProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnauthenticationoauthprofileclientrequiredclaim"></a>
# **DeleteMsgVpnAuthenticationOauthProfileClientRequiredClaim**
> SempMetaOnlyResponse DeleteMsgVpnAuthenticationOauthProfileClientRequiredClaim (string msgVpnName, string oauthProfileName, string clientRequiredClaimName)

Delete a Required Claim object.

Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Additional claims to be verified in the ID token.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnAuthenticationOauthProfileClientRequiredClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var clientRequiredClaimName = clientRequiredClaimName_example;  // string | The name of the ID token claim to verify.

            try
            {
                // Delete a Required Claim object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnAuthenticationOauthProfileClientRequiredClaim(msgVpnName, oauthProfileName, clientRequiredClaimName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.DeleteMsgVpnAuthenticationOauthProfileClientRequiredClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **clientRequiredClaimName** | **string**| The name of the ID token claim to verify. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="deletemsgvpnauthenticationoauthprofileresourceserverrequiredclaim"></a>
# **DeleteMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**
> SempMetaOnlyResponse DeleteMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim (string msgVpnName, string oauthProfileName, string resourceServerRequiredClaimName)

Delete a Required Claim object.

Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Additional claims to be verified in the access token.  A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class DeleteMsgVpnAuthenticationOauthProfileResourceServerRequiredClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var resourceServerRequiredClaimName = resourceServerRequiredClaimName_example;  // string | The name of the access token claim to verify.

            try
            {
                // Delete a Required Claim object.
                SempMetaOnlyResponse result = apiInstance.DeleteMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim(msgVpnName, oauthProfileName, resourceServerRequiredClaimName);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.DeleteMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **resourceServerRequiredClaimName** | **string**| The name of the access token claim to verify. | 

### Return type

[**SempMetaOnlyResponse**](SempMetaOnlyResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnauthenticationoauthprofile"></a>
# **GetMsgVpnAuthenticationOauthProfile**
> MsgVpnAuthenticationOauthProfileResponse GetMsgVpnAuthenticationOauthProfile (string msgVpnName, string oauthProfileName, string opaquePassword = null, List<string> select = null)

Get an OAuth Profile object.

Get an OAuth Profile object.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnAuthenticationOauthProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get an OAuth Profile object.
                MsgVpnAuthenticationOauthProfileResponse result = apiInstance.GetMsgVpnAuthenticationOauthProfile(msgVpnName, oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.GetMsgVpnAuthenticationOauthProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProfileResponse**](MsgVpnAuthenticationOauthProfileResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnauthenticationoauthprofileclientrequiredclaim"></a>
# **GetMsgVpnAuthenticationOauthProfileClientRequiredClaim**
> MsgVpnAuthenticationOauthProfileClientRequiredClaimResponse GetMsgVpnAuthenticationOauthProfileClientRequiredClaim (string msgVpnName, string oauthProfileName, string clientRequiredClaimName, string opaquePassword = null, List<string> select = null)

Get a Required Claim object.

Get a Required Claim object.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnAuthenticationOauthProfileClientRequiredClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var clientRequiredClaimName = clientRequiredClaimName_example;  // string | The name of the ID token claim to verify.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Required Claim object.
                MsgVpnAuthenticationOauthProfileClientRequiredClaimResponse result = apiInstance.GetMsgVpnAuthenticationOauthProfileClientRequiredClaim(msgVpnName, oauthProfileName, clientRequiredClaimName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.GetMsgVpnAuthenticationOauthProfileClientRequiredClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **clientRequiredClaimName** | **string**| The name of the ID token claim to verify. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProfileClientRequiredClaimResponse**](MsgVpnAuthenticationOauthProfileClientRequiredClaimResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnauthenticationoauthprofileclientrequiredclaims"></a>
# **GetMsgVpnAuthenticationOauthProfileClientRequiredClaims**
> MsgVpnAuthenticationOauthProfileClientRequiredClaimsResponse GetMsgVpnAuthenticationOauthProfileClientRequiredClaims (string msgVpnName, string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Required Claim objects.

Get a list of Required Claim objects.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnAuthenticationOauthProfileClientRequiredClaimsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Required Claim objects.
                MsgVpnAuthenticationOauthProfileClientRequiredClaimsResponse result = apiInstance.GetMsgVpnAuthenticationOauthProfileClientRequiredClaims(msgVpnName, oauthProfileName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.GetMsgVpnAuthenticationOauthProfileClientRequiredClaims: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProfileClientRequiredClaimsResponse**](MsgVpnAuthenticationOauthProfileClientRequiredClaimsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnauthenticationoauthprofileresourceserverrequiredclaim"></a>
# **GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**
> MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimResponse GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim (string msgVpnName, string oauthProfileName, string resourceServerRequiredClaimName, string opaquePassword = null, List<string> select = null)

Get a Required Claim object.

Get a Required Claim object.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaimExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var resourceServerRequiredClaimName = resourceServerRequiredClaimName_example;  // string | The name of the access token claim to verify.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a Required Claim object.
                MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimResponse result = apiInstance.GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim(msgVpnName, oauthProfileName, resourceServerRequiredClaimName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **resourceServerRequiredClaimName** | **string**| The name of the access token claim to verify. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimResponse**](MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnauthenticationoauthprofileresourceserverrequiredclaims"></a>
# **GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaims**
> MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimsResponse GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaims (string msgVpnName, string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of Required Claim objects.

Get a list of Required Claim objects.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaimsExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of Required Claim objects.
                MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimsResponse result = apiInstance.GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaims(msgVpnName, oauthProfileName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaims: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **count** | **int?**| Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. | [optional] [default to 10]
 **cursor** | **string**| The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. | [optional] 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **where** | [**List&lt;string&gt;**](string.md)| Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimsResponse**](MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimsResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="getmsgvpnauthenticationoauthprofiles"></a>
# **GetMsgVpnAuthenticationOauthProfiles**
> MsgVpnAuthenticationOauthProfilesResponse GetMsgVpnAuthenticationOauthProfiles (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)

Get a list of OAuth Profile objects.

Get a list of OAuth Profile objects.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-only\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class GetMsgVpnAuthenticationOauthProfilesExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var count = 56;  // int? | Limit the count of objects in the response. See the documentation for the `count` parameter. (optional)  (default to 10)
            var cursor = cursor_example;  // string | The cursor, or position, for the next page of objects. See the documentation for the `cursor` parameter. (optional) 
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var where = new List<string>(); // List<string> | Include in the response only objects where certain conditions are true. See the the documentation for the `where` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get a list of OAuth Profile objects.
                MsgVpnAuthenticationOauthProfilesResponse result = apiInstance.GetMsgVpnAuthenticationOauthProfiles(msgVpnName, count, cursor, opaquePassword, where, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.GetMsgVpnAuthenticationOauthProfiles: " + e.Message );
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

[**MsgVpnAuthenticationOauthProfilesResponse**](MsgVpnAuthenticationOauthProfilesResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: Not defined
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="replacemsgvpnauthenticationoauthprofile"></a>
# **ReplaceMsgVpnAuthenticationOauthProfile**
> MsgVpnAuthenticationOauthProfileResponse ReplaceMsgVpnAuthenticationOauthProfile (MsgVpnAuthenticationOauthProfile body, string msgVpnName, string oauthProfileName, string opaquePassword = null, List<string> select = null)

Replace an OAuth Profile object.

Replace an OAuth Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret||||x||||x msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class ReplaceMsgVpnAuthenticationOauthProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var body = new MsgVpnAuthenticationOauthProfile(); // MsgVpnAuthenticationOauthProfile | The OAuth Profile object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Replace an OAuth Profile object.
                MsgVpnAuthenticationOauthProfileResponse result = apiInstance.ReplaceMsgVpnAuthenticationOauthProfile(body, msgVpnName, oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.ReplaceMsgVpnAuthenticationOauthProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnAuthenticationOauthProfile**](MsgVpnAuthenticationOauthProfile.md)| The OAuth Profile object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProfileResponse**](MsgVpnAuthenticationOauthProfileResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
<a name="updatemsgvpnauthenticationoauthprofile"></a>
# **UpdateMsgVpnAuthenticationOauthProfile**
> MsgVpnAuthenticationOauthProfileResponse UpdateMsgVpnAuthenticationOauthProfile (MsgVpnAuthenticationOauthProfile body, string msgVpnName, string oauthProfileName, string opaquePassword = null, List<string> select = null)

Update an OAuth Profile object.

Update an OAuth Profile object. Any attribute missing from the request will be left unchanged.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret|||x||||x msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \"vpn/read-write\" is required to perform this operation.  This has been available since 2.25.

### Example
```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class UpdateMsgVpnAuthenticationOauthProfileExample
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AuthenticationOauthProfileApi();
            var body = new MsgVpnAuthenticationOauthProfile(); // MsgVpnAuthenticationOauthProfile | The OAuth Profile object's attributes.
            var msgVpnName = msgVpnName_example;  // string | The name of the Message VPN.
            var oauthProfileName = oauthProfileName_example;  // string | The name of the OAuth profile.
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Update an OAuth Profile object.
                MsgVpnAuthenticationOauthProfileResponse result = apiInstance.UpdateMsgVpnAuthenticationOauthProfile(body, msgVpnName, oauthProfileName, opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AuthenticationOauthProfileApi.UpdateMsgVpnAuthenticationOauthProfile: " + e.Message );
            }
        }
    }
}
```

### Parameters

Name | Type | Description  | Notes
------------- | ------------- | ------------- | -------------
 **body** | [**MsgVpnAuthenticationOauthProfile**](MsgVpnAuthenticationOauthProfile.md)| The OAuth Profile object&#x27;s attributes. | 
 **msgVpnName** | **string**| The name of the Message VPN. | 
 **oauthProfileName** | **string**| The name of the OAuth profile. | 
 **opaquePassword** | **string**| Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. | [optional] 
 **select** | [**List&lt;string&gt;**](string.md)| Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. | [optional] 

### Return type

[**MsgVpnAuthenticationOauthProfileResponse**](MsgVpnAuthenticationOauthProfileResponse.md)

### Authorization

[basicAuth](../README.md#basicAuth)

### HTTP request headers

 - **Content-Type**: application/json
 - **Accept**: application/json

[[Back to top]](#) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to Model list]](../README.md#documentation-for-models) [[Back to README]](../README.md)
