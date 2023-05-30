# Semp.V2.CSharp - the C# library for the SEMP (Solace Element Management Protocol)

SEMP (starting in `v2`, see note 1) is a RESTful API for configuring, monitoring, and administering a Solace PubSub+ broker.  SEMP uses URIs to address manageable **resources** of the Solace PubSub+ broker. Resources are individual **objects**, **collections** of objects, or (exclusively in the action API) **actions**. This document applies to the following API:   API|Base Path|Purpose|Comments :- --|:- --|:- --|:- -- Configuration|/SEMP/v2/config|Reading and writing config state|See note 2    The following APIs are also available:   API|Base Path|Purpose|Comments :- --|:- --|:- --|:- -- Action|/SEMP/v2/action|Performing actions|See note 2 Monitoring|/SEMP/v2/monitor|Querying operational parameters|See note 2    Resources are always nouns, with individual objects being singular and collections being plural.  Objects within a collection are identified by an `obj-id`, which follows the collection name with the form `collection-name/obj-id`.  Actions within an object are identified by an `action-id`, which follows the object name with the form `obj-id/action-id`.  Some examples:  ``` /SEMP/v2/config/msgVpns                        ; MsgVpn collection /SEMP/v2/config/msgVpns/a                      ; MsgVpn object named \"a\" /SEMP/v2/config/msgVpns/a/queues               ; Queue collection in MsgVpn \"a\" /SEMP/v2/config/msgVpns/a/queues/b             ; Queue object named \"b\" in MsgVpn \"a\" /SEMP/v2/action/msgVpns/a/queues/b/startReplay ; Action that starts a replay on Queue \"b\" in MsgVpn \"a\" /SEMP/v2/monitor/msgVpns/a/clients             ; Client collection in MsgVpn \"a\" /SEMP/v2/monitor/msgVpns/a/clients/c           ; Client object named \"c\" in MsgVpn \"a\" ```  ## Collection Resources  Collections are unordered lists of objects (unless described as otherwise), and are described by JSON arrays. Each item in the array represents an object in the same manner as the individual object would normally be represented. In the configuration API, the creation of a new object is done through its collection resource.  ## Object and Action Resources  Objects are composed of attributes, actions, collections, and other objects. They are described by JSON objects as name/value pairs. The collections and actions of an object are not contained directly in the object's JSON content; rather the content includes an attribute containing a URI which points to the collections and actions. These contained resources must be managed through this URI. At a minimum, every object has one or more identifying attributes, and its own `uri` attribute which contains the URI pointing to itself.  Actions are also composed of attributes, and are described by JSON objects as name/value pairs. Unlike objects, however, they are not members of a collection and cannot be retrieved, only performed. Actions only exist in the action API.  Attributes in an object or action may have any combination of the following properties:   Property|Meaning|Comments :- --|:- --|:- -- Identifying|Attribute is involved in unique identification of the object, and appears in its URI| Const|Attribute value can only be chosen during object creation| Required|Attribute must be provided in the request| Read-Only|Attribute can only be read, not written.|See note 3 Write-Only|Attribute can only be written, not read, unless the attribute is also opaque|See the documentation for the opaque property Requires-Disable|Attribute cannot be changed while the object (or the relevant part of the object) is administratively enabled| Auto-Disable|Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as one or more attributes will be temporarily disabled to apply the change| Deprecated|Attribute is deprecated, and will disappear in the next SEMP version| Opaque|Attribute can be set or retrieved in opaque form when the `opaquePassword` query parameter is present|See the `opaquePassword` query parameter documentation    In some requests, certain attributes may only be provided in certain combinations with other attributes:   Relationship|Meaning :- --|:- -- Requires|Attribute may only be changed by a request if a particular attribute or combination of attributes is also provided in the request Conflicts|Attribute may only be provided in a request if a particular attribute or combination of attributes is not also provided in the request    In the monitoring API, any non-identifying attribute may not be returned in a GET.  ## HTTP Methods  The following HTTP methods manipulate resources in accordance with these general principles. Note that some methods are only used in certain APIs:   Method|Resource|Meaning|Request Body|Response Body|Notes :- --|:- --|:- --|:- --|:- --|:- -- POST|Collection|Create object|Initial attribute values|Object attributes and metadata|Absent attributes are set to default. If object already exists, a 400 error is returned PUT|Object|Update object|New attribute values|Object attributes and metadata|If does not exist, the object is first created. Absent attributes are set to default, with certain exceptions (see note 4) PUT|Action|Performs action|Action arguments|Action metadata| PATCH|Object|Update object|New attribute values|Object attributes and metadata|Absent attributes are left unchanged. If the object does not exist, a 404 error is returned DELETE|Object|Delete object|Empty|Object metadata|If the object does not exist, a 404 is returned GET|Object|Get object|Empty|Object attributes and metadata|If the object does not exist, a 404 is returned GET|Collection|Get collection|Empty|Object attributes and collection metadata|If the collection is empty, then an empty collection is returned with a 200 code    ## Common Query Parameters  The following are some common query parameters that are supported by many method/URI combinations. Individual URIs may document additional parameters. Note that multiple query parameters can be used together in a single URI, separated by the ampersand character. For example:  ``` ; Request for the MsgVpns collection using two hypothetical query parameters ; \"q1\" and \"q2\" with values \"val1\" and \"val2\" respectively /SEMP/v2/config/msgVpns?q1=val1&q2=val2 ```  ### select  Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. Use this query parameter to limit the size of the returned data for each returned object, return only those fields that are desired, or exclude fields that are not desired.  The value of `select` is a comma-separated list of attribute names. If the list contains attribute names that are not prefaced by `-`, only those attributes are included in the response. If the list contains attribute names that are prefaced by `-`, those attributes are excluded from the response. If the list contains both types, then the difference of the first set of attributes and the second set of attributes is returned. If the list is empty (i.e. `select=`), it is treated the same as if no `select` was provided: all attribute are returned.  All attributes that are prefaced by `-` must follow all attributes that are not prefaced by `-`. In addition, each attribute name in the list must match at least one attribute in the object.  Names may include the `*` wildcard (zero or more characters). Nested attribute names are supported using periods (e.g. `parentName.childName`).  Some examples:  ``` ; List of all MsgVpn names /SEMP/v2/config/msgVpns?select=msgVpnName ; List of all MsgVpn and their attributes except for their names /SEMP/v2/config/msgVpns?select=-msgVpnName ; Authentication attributes of MsgVpn \"finance\" /SEMP/v2/config/msgVpns/finance?select=authentication%2A ; All attributes of MsgVpn \"finance\" except for authentication attributes /SEMP/v2/config/msgVpns/finance?select=-authentication%2A ; Access related attributes of Queue \"orderQ\" of MsgVpn \"finance\" /SEMP/v2/config/msgVpns/finance/queues/orderQ?select=owner,permission ```  ### where  Include in the response only objects where certain conditions are true. Use this query parameter to limit which objects are returned to those whose attribute values meet the given conditions.  The value of `where` is a comma-separated list of expressions. All expressions must be true for the object to be included in the response. Each expression takes the form:  ``` expression  = attribute-name OP value OP          = '==' | '!=' | '<' | '>' | '<=' | '>=' ```  `value` may be a number, string, `true`, or `false`, as appropriate for the type of `attribute-name`. Greater-than and less-than comparisons only work for numbers. A `*` in a string `value` is interpreted as a wildcard (zero or more characters). Some examples:  ``` ; Only enabled MsgVpns /SEMP/v2/config/msgVpns?where=enabled%3D%3Dtrue ; Only MsgVpns using basic non-LDAP authentication /SEMP/v2/config/msgVpns?where=authenticationBasicEnabled%3D%3Dtrue,authenticationBasicType%21%3Dldap ; Only MsgVpns that allow more than 100 client connections /SEMP/v2/config/msgVpns?where=maxConnectionCount%3E100 ; Only MsgVpns with msgVpnName starting with \"B\": /SEMP/v2/config/msgVpns?where=msgVpnName%3D%3DB%2A ```  ### count  Limit the count of objects in the response. This can be useful to limit the size of the response for large collections. The minimum value for `count` is `1` and the default is `10`. There is also a per-collection maximum value to limit request handling time.  `count` does not guarantee that a minimum number of objects will be returned. A page may contain fewer than `count` objects or even be empty. Additional objects may nonetheless be available for retrieval on subsequent pages. See the `cursor` query parameter documentation for more information on paging.  For example: ``` ; Up to 25 MsgVpns /SEMP/v2/config/msgVpns?count=25 ```  ### cursor  The cursor, or position, for the next page of objects. Cursors are opaque data that should not be created or interpreted by SEMP clients, and should only be used as described below.  When a request is made for a collection and there may be additional objects available for retrieval that are not included in the initial response, the response will include a `cursorQuery` field containing a cursor. The value of this field can be specified in the `cursor` query parameter of a subsequent request to retrieve the next page of objects.  Applications must continue to use the `cursorQuery` if one is provided in order to retrieve the full set of objects associated with the request, even if a page contains fewer than the requested number of objects (see the `count` query parameter documentation) or is empty.  ### opaquePassword  Attributes with the opaque property are also write-only and so cannot normally be retrieved in a GET. However, when a password is provided in the `opaquePassword` query parameter, attributes with the opaque property are retrieved in a GET in opaque form, encrypted with this password. The query parameter can also be used on a POST, PATCH, or PUT to set opaque attributes using opaque attribute values retrieved in a GET, so long as:  1. the same password that was used to retrieve the opaque attribute values is provided; and  2. the broker to which the request is being sent has the same major and minor SEMP version as the broker that produced the opaque attribute values.  The password provided in the query parameter must be a minimum of 8 characters and a maximum of 128 characters.  The query parameter can only be used in the configuration API, and only over HTTPS.  ## Authentication  When a client makes its first SEMPv2 request, it must supply a username and password using HTTP Basic authentication, or an OAuth token or tokens using HTTP Bearer authentication.  When HTTP Basic authentication is used, the broker returns a cookie containing a session key. The client can omit the username and password from subsequent requests, because the broker can use the session cookie for authentication instead. When the session expires or is deleted, the client must provide the username and password again, and the broker creates a new session.  There are a limited number of session slots available on the broker. The broker returns 529 No SEMP Session Available if it is not able to allocate a session.  If certain attributes—such as a user's password—are changed, the broker automatically deletes the affected sessions. These attributes are documented below. However, changes in external user configuration data stored on a RADIUS or LDAP server do not trigger the broker to delete the associated session(s), therefore you must do this manually, if required.  A client can retrieve its current session information using the /about/user endpoint and delete its own session using the /about/user/logout endpoint. A client with appropriate permissions can also manage all sessions using the /sessions endpoint.  Sessions are not created when authenticating with an OAuth token or tokens using HTTP Bearer authentication. If a session cookie is provided, it is ignored.  ## Help  Visit [our website](https://solace.com) to learn more about Solace.  You can also download the SEMP API specifications by clicking [here](https://solace.com/downloads/).  If you need additional support, please contact us at [support@solace.com](mailto:support@solace.com).  ## Notes  Note|Description :- --:|:- -- 1|This specification defines SEMP starting in \"v2\", and not the original SEMP \"v1\" interface. Request and response formats between \"v1\" and \"v2\" are entirely incompatible, although both protocols share a common port configuration on the Solace PubSub+ broker. They are differentiated by the initial portion of the URI path, one of either \"/SEMP/\" or \"/SEMP/v2/\" 2|This API is partially implemented. Only a subset of all objects are available. 3|Read-only attributes may appear in POST and PUT/PATCH requests. However, if a read-only attribute is not marked as identifying, it will be ignored during a PUT/PATCH. 4|On a PUT, if the SEMP user is not authorized to modify the attribute, its value is left unchanged rather than set to default. In addition, the values of write-only attributes are not set to their defaults on a PUT, except in the following two cases: there is a mutual requires relationship with another non-write-only attribute, both attributes are absent from the request, and the non-write-only attribute is not currently set to its default value; or the attribute is also opaque and the `opaquePassword` query parameter is provided in the request.  

This C# SDK is automatically generated by the [Swagger Codegen](https://github.com/swagger-api/swagger-codegen) project:

- API version: 2.35
- SDK version: 1.0.0
- Build package: io.swagger.codegen.v3.generators.dotnet.CSharpClientCodegen
    For more information, please visit [http://www.solace.com](http://www.solace.com)

<a name="frameworks-supported"></a>
## Frameworks supported
- .NET Core >=1.0
- .NET Framework >=4.6
- Mono/Xamarin >=vNext
- UWP >=10.0

<a name="dependencies"></a>
## Dependencies
- FubarCoder.RestSharp.Portable.Core >=4.0.7
- FubarCoder.RestSharp.Portable.HttpClient >=4.0.7
- Newtonsoft.Json >=10.0.3

<a name="installation"></a>
## Installation
Generate the DLL using your preferred tool

Then include the DLL (under the `bin` folder) in the C# project, and use the namespaces:
```csharp
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;
```
<a name="getting-started"></a>
## Getting Started

```csharp
using System;
using System.Diagnostics;
using Semp.V2.CSharp.Api;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Example
{
    public class Example
    {
        public void main()
        {
            // Configure HTTP basic authorization: basicAuth
            Configuration.Default.Username = "YOUR_USERNAME";
            Configuration.Default.Password = "YOUR_PASSWORD";

            var apiInstance = new AboutApi();
            var opaquePassword = opaquePassword_example;  // string | Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the `opaquePassword` parameter. (optional) 
            var select = new List<string>(); // List<string> | Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the `select` parameter. (optional) 

            try
            {
                // Get an About object.
                AboutResponse result = apiInstance.GetAbout(opaquePassword, select);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling AboutApi.GetAbout: " + e.Message );
            }
        }
    }
}
```

<a name="documentation-for-api-endpoints"></a>
## Documentation for API Endpoints

All URIs are relative to *http://www.solace.com/SEMP/v2/config*

Class | Method | HTTP request | Description
------------ | ------------- | ------------- | -------------
*AboutApi* | [**GetAbout**](docs/AboutApi.md#getabout) | **GET** /about | Get an About object.
*AboutApi* | [**GetAboutApi**](docs/AboutApi.md#getaboutapi) | **GET** /about/api | Get an API Description object.
*AboutApi* | [**GetAboutUser**](docs/AboutApi.md#getaboutuser) | **GET** /about/user | Get a User object.
*AboutApi* | [**GetAboutUserMsgVpn**](docs/AboutApi.md#getaboutusermsgvpn) | **GET** /about/user/msgVpns/{msgVpnName} | Get a User Message VPN object.
*AboutApi* | [**GetAboutUserMsgVpns**](docs/AboutApi.md#getaboutusermsgvpns) | **GET** /about/user/msgVpns | Get a list of User Message VPN objects.
*AclProfileApi* | [**CreateMsgVpnAclProfile**](docs/AclProfileApi.md#createmsgvpnaclprofile) | **POST** /msgVpns/{msgVpnName}/aclProfiles | Create an ACL Profile object.
*AclProfileApi* | [**CreateMsgVpnAclProfileClientConnectException**](docs/AclProfileApi.md#createmsgvpnaclprofileclientconnectexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions | Create a Client Connect Exception object.
*AclProfileApi* | [**CreateMsgVpnAclProfilePublishException**](docs/AclProfileApi.md#createmsgvpnaclprofilepublishexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions | Create a Publish Topic Exception object.
*AclProfileApi* | [**CreateMsgVpnAclProfilePublishTopicException**](docs/AclProfileApi.md#createmsgvpnaclprofilepublishtopicexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions | Create a Publish Topic Exception object.
*AclProfileApi* | [**CreateMsgVpnAclProfileSubscribeException**](docs/AclProfileApi.md#createmsgvpnaclprofilesubscribeexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions | Create a Subscribe Topic Exception object.
*AclProfileApi* | [**CreateMsgVpnAclProfileSubscribeShareNameException**](docs/AclProfileApi.md#createmsgvpnaclprofilesubscribesharenameexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions | Create a Subscribe Share Name Exception object.
*AclProfileApi* | [**CreateMsgVpnAclProfileSubscribeTopicException**](docs/AclProfileApi.md#createmsgvpnaclprofilesubscribetopicexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions | Create a Subscribe Topic Exception object.
*AclProfileApi* | [**DeleteMsgVpnAclProfile**](docs/AclProfileApi.md#deletemsgvpnaclprofile) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName} | Delete an ACL Profile object.
*AclProfileApi* | [**DeleteMsgVpnAclProfileClientConnectException**](docs/AclProfileApi.md#deletemsgvpnaclprofileclientconnectexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions/{clientConnectExceptionAddress} | Delete a Client Connect Exception object.
*AclProfileApi* | [**DeleteMsgVpnAclProfilePublishException**](docs/AclProfileApi.md#deletemsgvpnaclprofilepublishexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions/{topicSyntax},{publishExceptionTopic} | Delete a Publish Topic Exception object.
*AclProfileApi* | [**DeleteMsgVpnAclProfilePublishTopicException**](docs/AclProfileApi.md#deletemsgvpnaclprofilepublishtopicexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions/{publishTopicExceptionSyntax},{publishTopicException} | Delete a Publish Topic Exception object.
*AclProfileApi* | [**DeleteMsgVpnAclProfileSubscribeException**](docs/AclProfileApi.md#deletemsgvpnaclprofilesubscribeexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions/{topicSyntax},{subscribeExceptionTopic} | Delete a Subscribe Topic Exception object.
*AclProfileApi* | [**DeleteMsgVpnAclProfileSubscribeShareNameException**](docs/AclProfileApi.md#deletemsgvpnaclprofilesubscribesharenameexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions/{subscribeShareNameExceptionSyntax},{subscribeShareNameException} | Delete a Subscribe Share Name Exception object.
*AclProfileApi* | [**DeleteMsgVpnAclProfileSubscribeTopicException**](docs/AclProfileApi.md#deletemsgvpnaclprofilesubscribetopicexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions/{subscribeTopicExceptionSyntax},{subscribeTopicException} | Delete a Subscribe Topic Exception object.
*AclProfileApi* | [**GetMsgVpnAclProfile**](docs/AclProfileApi.md#getmsgvpnaclprofile) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName} | Get an ACL Profile object.
*AclProfileApi* | [**GetMsgVpnAclProfileClientConnectException**](docs/AclProfileApi.md#getmsgvpnaclprofileclientconnectexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions/{clientConnectExceptionAddress} | Get a Client Connect Exception object.
*AclProfileApi* | [**GetMsgVpnAclProfileClientConnectExceptions**](docs/AclProfileApi.md#getmsgvpnaclprofileclientconnectexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions | Get a list of Client Connect Exception objects.
*AclProfileApi* | [**GetMsgVpnAclProfilePublishException**](docs/AclProfileApi.md#getmsgvpnaclprofilepublishexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions/{topicSyntax},{publishExceptionTopic} | Get a Publish Topic Exception object.
*AclProfileApi* | [**GetMsgVpnAclProfilePublishExceptions**](docs/AclProfileApi.md#getmsgvpnaclprofilepublishexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions | Get a list of Publish Topic Exception objects.
*AclProfileApi* | [**GetMsgVpnAclProfilePublishTopicException**](docs/AclProfileApi.md#getmsgvpnaclprofilepublishtopicexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions/{publishTopicExceptionSyntax},{publishTopicException} | Get a Publish Topic Exception object.
*AclProfileApi* | [**GetMsgVpnAclProfilePublishTopicExceptions**](docs/AclProfileApi.md#getmsgvpnaclprofilepublishtopicexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions | Get a list of Publish Topic Exception objects.
*AclProfileApi* | [**GetMsgVpnAclProfileSubscribeException**](docs/AclProfileApi.md#getmsgvpnaclprofilesubscribeexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions/{topicSyntax},{subscribeExceptionTopic} | Get a Subscribe Topic Exception object.
*AclProfileApi* | [**GetMsgVpnAclProfileSubscribeExceptions**](docs/AclProfileApi.md#getmsgvpnaclprofilesubscribeexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions | Get a list of Subscribe Topic Exception objects.
*AclProfileApi* | [**GetMsgVpnAclProfileSubscribeShareNameException**](docs/AclProfileApi.md#getmsgvpnaclprofilesubscribesharenameexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions/{subscribeShareNameExceptionSyntax},{subscribeShareNameException} | Get a Subscribe Share Name Exception object.
*AclProfileApi* | [**GetMsgVpnAclProfileSubscribeShareNameExceptions**](docs/AclProfileApi.md#getmsgvpnaclprofilesubscribesharenameexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions | Get a list of Subscribe Share Name Exception objects.
*AclProfileApi* | [**GetMsgVpnAclProfileSubscribeTopicException**](docs/AclProfileApi.md#getmsgvpnaclprofilesubscribetopicexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions/{subscribeTopicExceptionSyntax},{subscribeTopicException} | Get a Subscribe Topic Exception object.
*AclProfileApi* | [**GetMsgVpnAclProfileSubscribeTopicExceptions**](docs/AclProfileApi.md#getmsgvpnaclprofilesubscribetopicexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions | Get a list of Subscribe Topic Exception objects.
*AclProfileApi* | [**GetMsgVpnAclProfiles**](docs/AclProfileApi.md#getmsgvpnaclprofiles) | **GET** /msgVpns/{msgVpnName}/aclProfiles | Get a list of ACL Profile objects.
*AclProfileApi* | [**ReplaceMsgVpnAclProfile**](docs/AclProfileApi.md#replacemsgvpnaclprofile) | **PUT** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName} | Replace an ACL Profile object.
*AclProfileApi* | [**UpdateMsgVpnAclProfile**](docs/AclProfileApi.md#updatemsgvpnaclprofile) | **PATCH** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName} | Update an ACL Profile object.
*AllApi* | [**CreateCertAuthority**](docs/AllApi.md#createcertauthority) | **POST** /certAuthorities | Create a Certificate Authority object.
*AllApi* | [**CreateCertAuthorityOcspTlsTrustedCommonName**](docs/AllApi.md#createcertauthorityocsptlstrustedcommonname) | **POST** /certAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames | Create an OCSP Responder Trusted Common Name object.
*AllApi* | [**CreateClientCertAuthority**](docs/AllApi.md#createclientcertauthority) | **POST** /clientCertAuthorities | Create a Client Certificate Authority object.
*AllApi* | [**CreateClientCertAuthorityOcspTlsTrustedCommonName**](docs/AllApi.md#createclientcertauthorityocsptlstrustedcommonname) | **POST** /clientCertAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames | Create an OCSP Responder Trusted Common Name object.
*AllApi* | [**CreateDmrCluster**](docs/AllApi.md#createdmrcluster) | **POST** /dmrClusters | Create a Cluster object.
*AllApi* | [**CreateDmrClusterCertMatchingRule**](docs/AllApi.md#createdmrclustercertmatchingrule) | **POST** /dmrClusters/{dmrClusterName}/certMatchingRules | Create a Certificate Matching Rule object.
*AllApi* | [**CreateDmrClusterCertMatchingRuleAttributeFilter**](docs/AllApi.md#createdmrclustercertmatchingruleattributefilter) | **POST** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters | Create a Certificate Matching Rule Attribute Filter object.
*AllApi* | [**CreateDmrClusterCertMatchingRuleCondition**](docs/AllApi.md#createdmrclustercertmatchingrulecondition) | **POST** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions | Create a Certificate Matching Rule Condition object.
*AllApi* | [**CreateDmrClusterLink**](docs/AllApi.md#createdmrclusterlink) | **POST** /dmrClusters/{dmrClusterName}/links | Create a Link object.
*AllApi* | [**CreateDmrClusterLinkAttribute**](docs/AllApi.md#createdmrclusterlinkattribute) | **POST** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes | Create a Link Attribute object.
*AllApi* | [**CreateDmrClusterLinkRemoteAddress**](docs/AllApi.md#createdmrclusterlinkremoteaddress) | **POST** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses | Create a Remote Address object.
*AllApi* | [**CreateDmrClusterLinkTlsTrustedCommonName**](docs/AllApi.md#createdmrclusterlinktlstrustedcommonname) | **POST** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames | Create a Trusted Common Name object.
*AllApi* | [**CreateDomainCertAuthority**](docs/AllApi.md#createdomaincertauthority) | **POST** /domainCertAuthorities | Create a Domain Certificate Authority object.
*AllApi* | [**CreateMsgVpn**](docs/AllApi.md#createmsgvpn) | **POST** /msgVpns | Create a Message VPN object.
*AllApi* | [**CreateMsgVpnAclProfile**](docs/AllApi.md#createmsgvpnaclprofile) | **POST** /msgVpns/{msgVpnName}/aclProfiles | Create an ACL Profile object.
*AllApi* | [**CreateMsgVpnAclProfileClientConnectException**](docs/AllApi.md#createmsgvpnaclprofileclientconnectexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions | Create a Client Connect Exception object.
*AllApi* | [**CreateMsgVpnAclProfilePublishException**](docs/AllApi.md#createmsgvpnaclprofilepublishexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions | Create a Publish Topic Exception object.
*AllApi* | [**CreateMsgVpnAclProfilePublishTopicException**](docs/AllApi.md#createmsgvpnaclprofilepublishtopicexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions | Create a Publish Topic Exception object.
*AllApi* | [**CreateMsgVpnAclProfileSubscribeException**](docs/AllApi.md#createmsgvpnaclprofilesubscribeexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions | Create a Subscribe Topic Exception object.
*AllApi* | [**CreateMsgVpnAclProfileSubscribeShareNameException**](docs/AllApi.md#createmsgvpnaclprofilesubscribesharenameexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions | Create a Subscribe Share Name Exception object.
*AllApi* | [**CreateMsgVpnAclProfileSubscribeTopicException**](docs/AllApi.md#createmsgvpnaclprofilesubscribetopicexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions | Create a Subscribe Topic Exception object.
*AllApi* | [**CreateMsgVpnAuthenticationOauthProfile**](docs/AllApi.md#createmsgvpnauthenticationoauthprofile) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProfiles | Create an OAuth Profile object.
*AllApi* | [**CreateMsgVpnAuthenticationOauthProfileClientRequiredClaim**](docs/AllApi.md#createmsgvpnauthenticationoauthprofileclientrequiredclaim) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims | Create a Required Claim object.
*AllApi* | [**CreateMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**](docs/AllApi.md#createmsgvpnauthenticationoauthprofileresourceserverrequiredclaim) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Create a Required Claim object.
*AllApi* | [**CreateMsgVpnAuthenticationOauthProvider**](docs/AllApi.md#createmsgvpnauthenticationoauthprovider) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProviders | Create an OAuth Provider object.
*AllApi* | [**CreateMsgVpnAuthorizationGroup**](docs/AllApi.md#createmsgvpnauthorizationgroup) | **POST** /msgVpns/{msgVpnName}/authorizationGroups | Create an Authorization Group object.
*AllApi* | [**CreateMsgVpnBridge**](docs/AllApi.md#createmsgvpnbridge) | **POST** /msgVpns/{msgVpnName}/bridges | Create a Bridge object.
*AllApi* | [**CreateMsgVpnBridgeRemoteMsgVpn**](docs/AllApi.md#createmsgvpnbridgeremotemsgvpn) | **POST** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns | Create a Remote Message VPN object.
*AllApi* | [**CreateMsgVpnBridgeRemoteSubscription**](docs/AllApi.md#createmsgvpnbridgeremotesubscription) | **POST** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions | Create a Remote Subscription object.
*AllApi* | [**CreateMsgVpnBridgeTlsTrustedCommonName**](docs/AllApi.md#createmsgvpnbridgetlstrustedcommonname) | **POST** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames | Create a Trusted Common Name object.
*AllApi* | [**CreateMsgVpnCertMatchingRule**](docs/AllApi.md#createmsgvpncertmatchingrule) | **POST** /msgVpns/{msgVpnName}/certMatchingRules | Create a Certificate Matching Rule object.
*AllApi* | [**CreateMsgVpnCertMatchingRuleAttributeFilter**](docs/AllApi.md#createmsgvpncertmatchingruleattributefilter) | **POST** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters | Create a Certificate Matching Rule Attribute Filter object.
*AllApi* | [**CreateMsgVpnCertMatchingRuleCondition**](docs/AllApi.md#createmsgvpncertmatchingrulecondition) | **POST** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions | Create a Certificate Matching Rule Condition object.
*AllApi* | [**CreateMsgVpnClientProfile**](docs/AllApi.md#createmsgvpnclientprofile) | **POST** /msgVpns/{msgVpnName}/clientProfiles | Create a Client Profile object.
*AllApi* | [**CreateMsgVpnClientUsername**](docs/AllApi.md#createmsgvpnclientusername) | **POST** /msgVpns/{msgVpnName}/clientUsernames | Create a Client Username object.
*AllApi* | [**CreateMsgVpnClientUsernameAttribute**](docs/AllApi.md#createmsgvpnclientusernameattribute) | **POST** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername}/attributes | Create a Client Username Attribute object.
*AllApi* | [**CreateMsgVpnDistributedCache**](docs/AllApi.md#createmsgvpndistributedcache) | **POST** /msgVpns/{msgVpnName}/distributedCaches | Create a Distributed Cache object.
*AllApi* | [**CreateMsgVpnDistributedCacheCluster**](docs/AllApi.md#createmsgvpndistributedcachecluster) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters | Create a Cache Cluster object.
*AllApi* | [**CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**](docs/AllApi.md#createmsgvpndistributedcacheclusterglobalcachinghomecluster) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters | Create a Home Cache Cluster object.
*AllApi* | [**CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**](docs/AllApi.md#createmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes | Create a Topic Prefix object.
*AllApi* | [**CreateMsgVpnDistributedCacheClusterInstance**](docs/AllApi.md#createmsgvpndistributedcacheclusterinstance) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances | Create a Cache Instance object.
*AllApi* | [**CreateMsgVpnDistributedCacheClusterTopic**](docs/AllApi.md#createmsgvpndistributedcacheclustertopic) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics | Create a Topic object.
*AllApi* | [**CreateMsgVpnDmrBridge**](docs/AllApi.md#createmsgvpndmrbridge) | **POST** /msgVpns/{msgVpnName}/dmrBridges | Create a DMR Bridge object.
*AllApi* | [**CreateMsgVpnJndiConnectionFactory**](docs/AllApi.md#createmsgvpnjndiconnectionfactory) | **POST** /msgVpns/{msgVpnName}/jndiConnectionFactories | Create a JNDI Connection Factory object.
*AllApi* | [**CreateMsgVpnJndiQueue**](docs/AllApi.md#createmsgvpnjndiqueue) | **POST** /msgVpns/{msgVpnName}/jndiQueues | Create a JNDI Queue object.
*AllApi* | [**CreateMsgVpnJndiTopic**](docs/AllApi.md#createmsgvpnjnditopic) | **POST** /msgVpns/{msgVpnName}/jndiTopics | Create a JNDI Topic object.
*AllApi* | [**CreateMsgVpnMqttRetainCache**](docs/AllApi.md#createmsgvpnmqttretaincache) | **POST** /msgVpns/{msgVpnName}/mqttRetainCaches | Create an MQTT Retain Cache object.
*AllApi* | [**CreateMsgVpnMqttSession**](docs/AllApi.md#createmsgvpnmqttsession) | **POST** /msgVpns/{msgVpnName}/mqttSessions | Create an MQTT Session object.
*AllApi* | [**CreateMsgVpnMqttSessionSubscription**](docs/AllApi.md#createmsgvpnmqttsessionsubscription) | **POST** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions | Create a Subscription object.
*AllApi* | [**CreateMsgVpnQueue**](docs/AllApi.md#createmsgvpnqueue) | **POST** /msgVpns/{msgVpnName}/queues | Create a Queue object.
*AllApi* | [**CreateMsgVpnQueueSubscription**](docs/AllApi.md#createmsgvpnqueuesubscription) | **POST** /msgVpns/{msgVpnName}/queues/{queueName}/subscriptions | Create a Queue Subscription object.
*AllApi* | [**CreateMsgVpnQueueTemplate**](docs/AllApi.md#createmsgvpnqueuetemplate) | **POST** /msgVpns/{msgVpnName}/queueTemplates | Create a Queue Template object.
*AllApi* | [**CreateMsgVpnReplayLog**](docs/AllApi.md#createmsgvpnreplaylog) | **POST** /msgVpns/{msgVpnName}/replayLogs | Create a Replay Log object.
*AllApi* | [**CreateMsgVpnReplayLogTopicFilterSubscription**](docs/AllApi.md#createmsgvpnreplaylogtopicfiltersubscription) | **POST** /msgVpns/{msgVpnName}/replayLogs/{replayLogName}/topicFilterSubscriptions | Create a Topic Filter Subscription object.
*AllApi* | [**CreateMsgVpnReplicatedTopic**](docs/AllApi.md#createmsgvpnreplicatedtopic) | **POST** /msgVpns/{msgVpnName}/replicatedTopics | Create a Replicated Topic object.
*AllApi* | [**CreateMsgVpnRestDeliveryPoint**](docs/AllApi.md#createmsgvpnrestdeliverypoint) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints | Create a REST Delivery Point object.
*AllApi* | [**CreateMsgVpnRestDeliveryPointQueueBinding**](docs/AllApi.md#createmsgvpnrestdeliverypointqueuebinding) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings | Create a Queue Binding object.
*AllApi* | [**CreateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/AllApi.md#createmsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders | Create a Protected Request Header object.
*AllApi* | [**CreateMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/AllApi.md#createmsgvpnrestdeliverypointqueuebindingrequestheader) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders | Create a Request Header object.
*AllApi* | [**CreateMsgVpnRestDeliveryPointRestConsumer**](docs/AllApi.md#createmsgvpnrestdeliverypointrestconsumer) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers | Create a REST Consumer object.
*AllApi* | [**CreateMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**](docs/AllApi.md#createmsgvpnrestdeliverypointrestconsumeroauthjwtclaim) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims | Create a Claim object.
*AllApi* | [**CreateMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**](docs/AllApi.md#createmsgvpnrestdeliverypointrestconsumertlstrustedcommonname) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames | Create a Trusted Common Name object.
*AllApi* | [**CreateMsgVpnSequencedTopic**](docs/AllApi.md#createmsgvpnsequencedtopic) | **POST** /msgVpns/{msgVpnName}/sequencedTopics | Create a Sequenced Topic object.
*AllApi* | [**CreateMsgVpnTelemetryProfile**](docs/AllApi.md#createmsgvpntelemetryprofile) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles | Create a Telemetry Profile object.
*AllApi* | [**CreateMsgVpnTelemetryProfileReceiverAclConnectException**](docs/AllApi.md#createmsgvpntelemetryprofilereceiveraclconnectexception) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions | Create a Receiver ACL Connect Exception object.
*AllApi* | [**CreateMsgVpnTelemetryProfileTraceFilter**](docs/AllApi.md#createmsgvpntelemetryprofiletracefilter) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters | Create a Trace Filter object.
*AllApi* | [**CreateMsgVpnTelemetryProfileTraceFilterSubscription**](docs/AllApi.md#createmsgvpntelemetryprofiletracefiltersubscription) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions | Create a Telemetry Trace Filter Subscription object.
*AllApi* | [**CreateMsgVpnTopicEndpoint**](docs/AllApi.md#createmsgvpntopicendpoint) | **POST** /msgVpns/{msgVpnName}/topicEndpoints | Create a Topic Endpoint object.
*AllApi* | [**CreateMsgVpnTopicEndpointTemplate**](docs/AllApi.md#createmsgvpntopicendpointtemplate) | **POST** /msgVpns/{msgVpnName}/topicEndpointTemplates | Create a Topic Endpoint Template object.
*AllApi* | [**CreateOauthProfile**](docs/AllApi.md#createoauthprofile) | **POST** /oauthProfiles | Create an OAuth Profile object.
*AllApi* | [**CreateOauthProfileAccessLevelGroup**](docs/AllApi.md#createoauthprofileaccesslevelgroup) | **POST** /oauthProfiles/{oauthProfileName}/accessLevelGroups | Create a Group Access Level object.
*AllApi* | [**CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](docs/AllApi.md#createoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **POST** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions | Create a Message VPN Access-Level Exception object.
*AllApi* | [**CreateOauthProfileClientAllowedHost**](docs/AllApi.md#createoauthprofileclientallowedhost) | **POST** /oauthProfiles/{oauthProfileName}/clientAllowedHosts | Create an Allowed Host Value object.
*AllApi* | [**CreateOauthProfileClientAuthorizationParameter**](docs/AllApi.md#createoauthprofileclientauthorizationparameter) | **POST** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters | Create an Authorization Parameter object.
*AllApi* | [**CreateOauthProfileClientRequiredClaim**](docs/AllApi.md#createoauthprofileclientrequiredclaim) | **POST** /oauthProfiles/{oauthProfileName}/clientRequiredClaims | Create a Required Claim object.
*AllApi* | [**CreateOauthProfileDefaultMsgVpnAccessLevelException**](docs/AllApi.md#createoauthprofiledefaultmsgvpnaccesslevelexception) | **POST** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions | Create a Message VPN Access-Level Exception object.
*AllApi* | [**CreateOauthProfileResourceServerRequiredClaim**](docs/AllApi.md#createoauthprofileresourceserverrequiredclaim) | **POST** /oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Create a Required Claim object.
*AllApi* | [**CreateVirtualHostname**](docs/AllApi.md#createvirtualhostname) | **POST** /virtualHostnames | Create a Virtual Hostname object.
*AllApi* | [**DeleteCertAuthority**](docs/AllApi.md#deletecertauthority) | **DELETE** /certAuthorities/{certAuthorityName} | Delete a Certificate Authority object.
*AllApi* | [**DeleteCertAuthorityOcspTlsTrustedCommonName**](docs/AllApi.md#deletecertauthorityocsptlstrustedcommonname) | **DELETE** /certAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames/{ocspTlsTrustedCommonName} | Delete an OCSP Responder Trusted Common Name object.
*AllApi* | [**DeleteClientCertAuthority**](docs/AllApi.md#deleteclientcertauthority) | **DELETE** /clientCertAuthorities/{certAuthorityName} | Delete a Client Certificate Authority object.
*AllApi* | [**DeleteClientCertAuthorityOcspTlsTrustedCommonName**](docs/AllApi.md#deleteclientcertauthorityocsptlstrustedcommonname) | **DELETE** /clientCertAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames/{ocspTlsTrustedCommonName} | Delete an OCSP Responder Trusted Common Name object.
*AllApi* | [**DeleteDmrCluster**](docs/AllApi.md#deletedmrcluster) | **DELETE** /dmrClusters/{dmrClusterName} | Delete a Cluster object.
*AllApi* | [**DeleteDmrClusterCertMatchingRule**](docs/AllApi.md#deletedmrclustercertmatchingrule) | **DELETE** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName} | Delete a Certificate Matching Rule object.
*AllApi* | [**DeleteDmrClusterCertMatchingRuleAttributeFilter**](docs/AllApi.md#deletedmrclustercertmatchingruleattributefilter) | **DELETE** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Delete a Certificate Matching Rule Attribute Filter object.
*AllApi* | [**DeleteDmrClusterCertMatchingRuleCondition**](docs/AllApi.md#deletedmrclustercertmatchingrulecondition) | **DELETE** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions/{source} | Delete a Certificate Matching Rule Condition object.
*AllApi* | [**DeleteDmrClusterLink**](docs/AllApi.md#deletedmrclusterlink) | **DELETE** /dmrClusters/{dmrClusterName}/links/{remoteNodeName} | Delete a Link object.
*AllApi* | [**DeleteDmrClusterLinkAttribute**](docs/AllApi.md#deletedmrclusterlinkattribute) | **DELETE** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes/{attributeName},{attributeValue} | Delete a Link Attribute object.
*AllApi* | [**DeleteDmrClusterLinkRemoteAddress**](docs/AllApi.md#deletedmrclusterlinkremoteaddress) | **DELETE** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses/{remoteAddress} | Delete a Remote Address object.
*AllApi* | [**DeleteDmrClusterLinkTlsTrustedCommonName**](docs/AllApi.md#deletedmrclusterlinktlstrustedcommonname) | **DELETE** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Delete a Trusted Common Name object.
*AllApi* | [**DeleteDomainCertAuthority**](docs/AllApi.md#deletedomaincertauthority) | **DELETE** /domainCertAuthorities/{certAuthorityName} | Delete a Domain Certificate Authority object.
*AllApi* | [**DeleteMsgVpn**](docs/AllApi.md#deletemsgvpn) | **DELETE** /msgVpns/{msgVpnName} | Delete a Message VPN object.
*AllApi* | [**DeleteMsgVpnAclProfile**](docs/AllApi.md#deletemsgvpnaclprofile) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName} | Delete an ACL Profile object.
*AllApi* | [**DeleteMsgVpnAclProfileClientConnectException**](docs/AllApi.md#deletemsgvpnaclprofileclientconnectexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions/{clientConnectExceptionAddress} | Delete a Client Connect Exception object.
*AllApi* | [**DeleteMsgVpnAclProfilePublishException**](docs/AllApi.md#deletemsgvpnaclprofilepublishexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions/{topicSyntax},{publishExceptionTopic} | Delete a Publish Topic Exception object.
*AllApi* | [**DeleteMsgVpnAclProfilePublishTopicException**](docs/AllApi.md#deletemsgvpnaclprofilepublishtopicexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions/{publishTopicExceptionSyntax},{publishTopicException} | Delete a Publish Topic Exception object.
*AllApi* | [**DeleteMsgVpnAclProfileSubscribeException**](docs/AllApi.md#deletemsgvpnaclprofilesubscribeexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions/{topicSyntax},{subscribeExceptionTopic} | Delete a Subscribe Topic Exception object.
*AllApi* | [**DeleteMsgVpnAclProfileSubscribeShareNameException**](docs/AllApi.md#deletemsgvpnaclprofilesubscribesharenameexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions/{subscribeShareNameExceptionSyntax},{subscribeShareNameException} | Delete a Subscribe Share Name Exception object.
*AllApi* | [**DeleteMsgVpnAclProfileSubscribeTopicException**](docs/AllApi.md#deletemsgvpnaclprofilesubscribetopicexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions/{subscribeTopicExceptionSyntax},{subscribeTopicException} | Delete a Subscribe Topic Exception object.
*AllApi* | [**DeleteMsgVpnAuthenticationOauthProfile**](docs/AllApi.md#deletemsgvpnauthenticationoauthprofile) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Delete an OAuth Profile object.
*AllApi* | [**DeleteMsgVpnAuthenticationOauthProfileClientRequiredClaim**](docs/AllApi.md#deletemsgvpnauthenticationoauthprofileclientrequiredclaim) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Delete a Required Claim object.
*AllApi* | [**DeleteMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**](docs/AllApi.md#deletemsgvpnauthenticationoauthprofileresourceserverrequiredclaim) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Delete a Required Claim object.
*AllApi* | [**DeleteMsgVpnAuthenticationOauthProvider**](docs/AllApi.md#deletemsgvpnauthenticationoauthprovider) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Delete an OAuth Provider object.
*AllApi* | [**DeleteMsgVpnAuthorizationGroup**](docs/AllApi.md#deletemsgvpnauthorizationgroup) | **DELETE** /msgVpns/{msgVpnName}/authorizationGroups/{authorizationGroupName} | Delete an Authorization Group object.
*AllApi* | [**DeleteMsgVpnBridge**](docs/AllApi.md#deletemsgvpnbridge) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Delete a Bridge object.
*AllApi* | [**DeleteMsgVpnBridgeRemoteMsgVpn**](docs/AllApi.md#deletemsgvpnbridgeremotemsgvpn) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Delete a Remote Message VPN object.
*AllApi* | [**DeleteMsgVpnBridgeRemoteSubscription**](docs/AllApi.md#deletemsgvpnbridgeremotesubscription) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions/{remoteSubscriptionTopic} | Delete a Remote Subscription object.
*AllApi* | [**DeleteMsgVpnBridgeTlsTrustedCommonName**](docs/AllApi.md#deletemsgvpnbridgetlstrustedcommonname) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Delete a Trusted Common Name object.
*AllApi* | [**DeleteMsgVpnCertMatchingRule**](docs/AllApi.md#deletemsgvpncertmatchingrule) | **DELETE** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Delete a Certificate Matching Rule object.
*AllApi* | [**DeleteMsgVpnCertMatchingRuleAttributeFilter**](docs/AllApi.md#deletemsgvpncertmatchingruleattributefilter) | **DELETE** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Delete a Certificate Matching Rule Attribute Filter object.
*AllApi* | [**DeleteMsgVpnCertMatchingRuleCondition**](docs/AllApi.md#deletemsgvpncertmatchingrulecondition) | **DELETE** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions/{source} | Delete a Certificate Matching Rule Condition object.
*AllApi* | [**DeleteMsgVpnClientProfile**](docs/AllApi.md#deletemsgvpnclientprofile) | **DELETE** /msgVpns/{msgVpnName}/clientProfiles/{clientProfileName} | Delete a Client Profile object.
*AllApi* | [**DeleteMsgVpnClientUsername**](docs/AllApi.md#deletemsgvpnclientusername) | **DELETE** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername} | Delete a Client Username object.
*AllApi* | [**DeleteMsgVpnClientUsernameAttribute**](docs/AllApi.md#deletemsgvpnclientusernameattribute) | **DELETE** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername}/attributes/{attributeName},{attributeValue} | Delete a Client Username Attribute object.
*AllApi* | [**DeleteMsgVpnDistributedCache**](docs/AllApi.md#deletemsgvpndistributedcache) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Delete a Distributed Cache object.
*AllApi* | [**DeleteMsgVpnDistributedCacheCluster**](docs/AllApi.md#deletemsgvpndistributedcachecluster) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Delete a Cache Cluster object.
*AllApi* | [**DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**](docs/AllApi.md#deletemsgvpndistributedcacheclusterglobalcachinghomecluster) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName} | Delete a Home Cache Cluster object.
*AllApi* | [**DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**](docs/AllApi.md#deletemsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes/{topicPrefix} | Delete a Topic Prefix object.
*AllApi* | [**DeleteMsgVpnDistributedCacheClusterInstance**](docs/AllApi.md#deletemsgvpndistributedcacheclusterinstance) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Delete a Cache Instance object.
*AllApi* | [**DeleteMsgVpnDistributedCacheClusterTopic**](docs/AllApi.md#deletemsgvpndistributedcacheclustertopic) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics/{topic} | Delete a Topic object.
*AllApi* | [**DeleteMsgVpnDmrBridge**](docs/AllApi.md#deletemsgvpndmrbridge) | **DELETE** /msgVpns/{msgVpnName}/dmrBridges/{remoteNodeName} | Delete a DMR Bridge object.
*AllApi* | [**DeleteMsgVpnJndiConnectionFactory**](docs/AllApi.md#deletemsgvpnjndiconnectionfactory) | **DELETE** /msgVpns/{msgVpnName}/jndiConnectionFactories/{connectionFactoryName} | Delete a JNDI Connection Factory object.
*AllApi* | [**DeleteMsgVpnJndiQueue**](docs/AllApi.md#deletemsgvpnjndiqueue) | **DELETE** /msgVpns/{msgVpnName}/jndiQueues/{queueName} | Delete a JNDI Queue object.
*AllApi* | [**DeleteMsgVpnJndiTopic**](docs/AllApi.md#deletemsgvpnjnditopic) | **DELETE** /msgVpns/{msgVpnName}/jndiTopics/{topicName} | Delete a JNDI Topic object.
*AllApi* | [**DeleteMsgVpnMqttRetainCache**](docs/AllApi.md#deletemsgvpnmqttretaincache) | **DELETE** /msgVpns/{msgVpnName}/mqttRetainCaches/{cacheName} | Delete an MQTT Retain Cache object.
*AllApi* | [**DeleteMsgVpnMqttSession**](docs/AllApi.md#deletemsgvpnmqttsession) | **DELETE** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter} | Delete an MQTT Session object.
*AllApi* | [**DeleteMsgVpnMqttSessionSubscription**](docs/AllApi.md#deletemsgvpnmqttsessionsubscription) | **DELETE** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions/{subscriptionTopic} | Delete a Subscription object.
*AllApi* | [**DeleteMsgVpnQueue**](docs/AllApi.md#deletemsgvpnqueue) | **DELETE** /msgVpns/{msgVpnName}/queues/{queueName} | Delete a Queue object.
*AllApi* | [**DeleteMsgVpnQueueSubscription**](docs/AllApi.md#deletemsgvpnqueuesubscription) | **DELETE** /msgVpns/{msgVpnName}/queues/{queueName}/subscriptions/{subscriptionTopic} | Delete a Queue Subscription object.
*AllApi* | [**DeleteMsgVpnQueueTemplate**](docs/AllApi.md#deletemsgvpnqueuetemplate) | **DELETE** /msgVpns/{msgVpnName}/queueTemplates/{queueTemplateName} | Delete a Queue Template object.
*AllApi* | [**DeleteMsgVpnReplayLog**](docs/AllApi.md#deletemsgvpnreplaylog) | **DELETE** /msgVpns/{msgVpnName}/replayLogs/{replayLogName} | Delete a Replay Log object.
*AllApi* | [**DeleteMsgVpnReplayLogTopicFilterSubscription**](docs/AllApi.md#deletemsgvpnreplaylogtopicfiltersubscription) | **DELETE** /msgVpns/{msgVpnName}/replayLogs/{replayLogName}/topicFilterSubscriptions/{topicFilterSubscription} | Delete a Topic Filter Subscription object.
*AllApi* | [**DeleteMsgVpnReplicatedTopic**](docs/AllApi.md#deletemsgvpnreplicatedtopic) | **DELETE** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Delete a Replicated Topic object.
*AllApi* | [**DeleteMsgVpnRestDeliveryPoint**](docs/AllApi.md#deletemsgvpnrestdeliverypoint) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Delete a REST Delivery Point object.
*AllApi* | [**DeleteMsgVpnRestDeliveryPointQueueBinding**](docs/AllApi.md#deletemsgvpnrestdeliverypointqueuebinding) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Delete a Queue Binding object.
*AllApi* | [**DeleteMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/AllApi.md#deletemsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Delete a Protected Request Header object.
*AllApi* | [**DeleteMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/AllApi.md#deletemsgvpnrestdeliverypointqueuebindingrequestheader) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Delete a Request Header object.
*AllApi* | [**DeleteMsgVpnRestDeliveryPointRestConsumer**](docs/AllApi.md#deletemsgvpnrestdeliverypointrestconsumer) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Delete a REST Consumer object.
*AllApi* | [**DeleteMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**](docs/AllApi.md#deletemsgvpnrestdeliverypointrestconsumeroauthjwtclaim) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims/{oauthJwtClaimName} | Delete a Claim object.
*AllApi* | [**DeleteMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**](docs/AllApi.md#deletemsgvpnrestdeliverypointrestconsumertlstrustedcommonname) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Delete a Trusted Common Name object.
*AllApi* | [**DeleteMsgVpnSequencedTopic**](docs/AllApi.md#deletemsgvpnsequencedtopic) | **DELETE** /msgVpns/{msgVpnName}/sequencedTopics/{sequencedTopic} | Delete a Sequenced Topic object.
*AllApi* | [**DeleteMsgVpnTelemetryProfile**](docs/AllApi.md#deletemsgvpntelemetryprofile) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Delete a Telemetry Profile object.
*AllApi* | [**DeleteMsgVpnTelemetryProfileReceiverAclConnectException**](docs/AllApi.md#deletemsgvpntelemetryprofilereceiveraclconnectexception) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions/{receiverAclConnectExceptionAddress} | Delete a Receiver ACL Connect Exception object.
*AllApi* | [**DeleteMsgVpnTelemetryProfileTraceFilter**](docs/AllApi.md#deletemsgvpntelemetryprofiletracefilter) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Delete a Trace Filter object.
*AllApi* | [**DeleteMsgVpnTelemetryProfileTraceFilterSubscription**](docs/AllApi.md#deletemsgvpntelemetryprofiletracefiltersubscription) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions/{subscription},{subscriptionSyntax} | Delete a Telemetry Trace Filter Subscription object.
*AllApi* | [**DeleteMsgVpnTopicEndpoint**](docs/AllApi.md#deletemsgvpntopicendpoint) | **DELETE** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Delete a Topic Endpoint object.
*AllApi* | [**DeleteMsgVpnTopicEndpointTemplate**](docs/AllApi.md#deletemsgvpntopicendpointtemplate) | **DELETE** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Delete a Topic Endpoint Template object.
*AllApi* | [**DeleteOauthProfile**](docs/AllApi.md#deleteoauthprofile) | **DELETE** /oauthProfiles/{oauthProfileName} | Delete an OAuth Profile object.
*AllApi* | [**DeleteOauthProfileAccessLevelGroup**](docs/AllApi.md#deleteoauthprofileaccesslevelgroup) | **DELETE** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName} | Delete a Group Access Level object.
*AllApi* | [**DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](docs/AllApi.md#deleteoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **DELETE** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName} | Delete a Message VPN Access-Level Exception object.
*AllApi* | [**DeleteOauthProfileClientAllowedHost**](docs/AllApi.md#deleteoauthprofileclientallowedhost) | **DELETE** /oauthProfiles/{oauthProfileName}/clientAllowedHosts/{allowedHost} | Delete an Allowed Host Value object.
*AllApi* | [**DeleteOauthProfileClientAuthorizationParameter**](docs/AllApi.md#deleteoauthprofileclientauthorizationparameter) | **DELETE** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName} | Delete an Authorization Parameter object.
*AllApi* | [**DeleteOauthProfileClientRequiredClaim**](docs/AllApi.md#deleteoauthprofileclientrequiredclaim) | **DELETE** /oauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Delete a Required Claim object.
*AllApi* | [**DeleteOauthProfileDefaultMsgVpnAccessLevelException**](docs/AllApi.md#deleteoauthprofiledefaultmsgvpnaccesslevelexception) | **DELETE** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName} | Delete a Message VPN Access-Level Exception object.
*AllApi* | [**DeleteOauthProfileResourceServerRequiredClaim**](docs/AllApi.md#deleteoauthprofileresourceserverrequiredclaim) | **DELETE** /oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Delete a Required Claim object.
*AllApi* | [**DeleteVirtualHostname**](docs/AllApi.md#deletevirtualhostname) | **DELETE** /virtualHostnames/{virtualHostname} | Delete a Virtual Hostname object.
*AllApi* | [**GetAbout**](docs/AllApi.md#getabout) | **GET** /about | Get an About object.
*AllApi* | [**GetAboutApi**](docs/AllApi.md#getaboutapi) | **GET** /about/api | Get an API Description object.
*AllApi* | [**GetAboutUser**](docs/AllApi.md#getaboutuser) | **GET** /about/user | Get a User object.
*AllApi* | [**GetAboutUserMsgVpn**](docs/AllApi.md#getaboutusermsgvpn) | **GET** /about/user/msgVpns/{msgVpnName} | Get a User Message VPN object.
*AllApi* | [**GetAboutUserMsgVpns**](docs/AllApi.md#getaboutusermsgvpns) | **GET** /about/user/msgVpns | Get a list of User Message VPN objects.
*AllApi* | [**GetBroker**](docs/AllApi.md#getbroker) | **GET** / | Get a Broker object.
*AllApi* | [**GetCertAuthorities**](docs/AllApi.md#getcertauthorities) | **GET** /certAuthorities | Get a list of Certificate Authority objects.
*AllApi* | [**GetCertAuthority**](docs/AllApi.md#getcertauthority) | **GET** /certAuthorities/{certAuthorityName} | Get a Certificate Authority object.
*AllApi* | [**GetCertAuthorityOcspTlsTrustedCommonName**](docs/AllApi.md#getcertauthorityocsptlstrustedcommonname) | **GET** /certAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames/{ocspTlsTrustedCommonName} | Get an OCSP Responder Trusted Common Name object.
*AllApi* | [**GetCertAuthorityOcspTlsTrustedCommonNames**](docs/AllApi.md#getcertauthorityocsptlstrustedcommonnames) | **GET** /certAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames | Get a list of OCSP Responder Trusted Common Name objects.
*AllApi* | [**GetClientCertAuthorities**](docs/AllApi.md#getclientcertauthorities) | **GET** /clientCertAuthorities | Get a list of Client Certificate Authority objects.
*AllApi* | [**GetClientCertAuthority**](docs/AllApi.md#getclientcertauthority) | **GET** /clientCertAuthorities/{certAuthorityName} | Get a Client Certificate Authority object.
*AllApi* | [**GetClientCertAuthorityOcspTlsTrustedCommonName**](docs/AllApi.md#getclientcertauthorityocsptlstrustedcommonname) | **GET** /clientCertAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames/{ocspTlsTrustedCommonName} | Get an OCSP Responder Trusted Common Name object.
*AllApi* | [**GetClientCertAuthorityOcspTlsTrustedCommonNames**](docs/AllApi.md#getclientcertauthorityocsptlstrustedcommonnames) | **GET** /clientCertAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames | Get a list of OCSP Responder Trusted Common Name objects.
*AllApi* | [**GetDmrCluster**](docs/AllApi.md#getdmrcluster) | **GET** /dmrClusters/{dmrClusterName} | Get a Cluster object.
*AllApi* | [**GetDmrClusterCertMatchingRule**](docs/AllApi.md#getdmrclustercertmatchingrule) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName} | Get a Certificate Matching Rule object.
*AllApi* | [**GetDmrClusterCertMatchingRuleAttributeFilter**](docs/AllApi.md#getdmrclustercertmatchingruleattributefilter) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Get a Certificate Matching Rule Attribute Filter object.
*AllApi* | [**GetDmrClusterCertMatchingRuleAttributeFilters**](docs/AllApi.md#getdmrclustercertmatchingruleattributefilters) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters | Get a list of Certificate Matching Rule Attribute Filter objects.
*AllApi* | [**GetDmrClusterCertMatchingRuleCondition**](docs/AllApi.md#getdmrclustercertmatchingrulecondition) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions/{source} | Get a Certificate Matching Rule Condition object.
*AllApi* | [**GetDmrClusterCertMatchingRuleConditions**](docs/AllApi.md#getdmrclustercertmatchingruleconditions) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions | Get a list of Certificate Matching Rule Condition objects.
*AllApi* | [**GetDmrClusterCertMatchingRules**](docs/AllApi.md#getdmrclustercertmatchingrules) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules | Get a list of Certificate Matching Rule objects.
*AllApi* | [**GetDmrClusterLink**](docs/AllApi.md#getdmrclusterlink) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName} | Get a Link object.
*AllApi* | [**GetDmrClusterLinkAttribute**](docs/AllApi.md#getdmrclusterlinkattribute) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes/{attributeName},{attributeValue} | Get a Link Attribute object.
*AllApi* | [**GetDmrClusterLinkAttributes**](docs/AllApi.md#getdmrclusterlinkattributes) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes | Get a list of Link Attribute objects.
*AllApi* | [**GetDmrClusterLinkRemoteAddress**](docs/AllApi.md#getdmrclusterlinkremoteaddress) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses/{remoteAddress} | Get a Remote Address object.
*AllApi* | [**GetDmrClusterLinkRemoteAddresses**](docs/AllApi.md#getdmrclusterlinkremoteaddresses) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses | Get a list of Remote Address objects.
*AllApi* | [**GetDmrClusterLinkTlsTrustedCommonName**](docs/AllApi.md#getdmrclusterlinktlstrustedcommonname) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Get a Trusted Common Name object.
*AllApi* | [**GetDmrClusterLinkTlsTrustedCommonNames**](docs/AllApi.md#getdmrclusterlinktlstrustedcommonnames) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames | Get a list of Trusted Common Name objects.
*AllApi* | [**GetDmrClusterLinks**](docs/AllApi.md#getdmrclusterlinks) | **GET** /dmrClusters/{dmrClusterName}/links | Get a list of Link objects.
*AllApi* | [**GetDmrClusters**](docs/AllApi.md#getdmrclusters) | **GET** /dmrClusters | Get a list of Cluster objects.
*AllApi* | [**GetDomainCertAuthorities**](docs/AllApi.md#getdomaincertauthorities) | **GET** /domainCertAuthorities | Get a list of Domain Certificate Authority objects.
*AllApi* | [**GetDomainCertAuthority**](docs/AllApi.md#getdomaincertauthority) | **GET** /domainCertAuthorities/{certAuthorityName} | Get a Domain Certificate Authority object.
*AllApi* | [**GetMsgVpn**](docs/AllApi.md#getmsgvpn) | **GET** /msgVpns/{msgVpnName} | Get a Message VPN object.
*AllApi* | [**GetMsgVpnAclProfile**](docs/AllApi.md#getmsgvpnaclprofile) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName} | Get an ACL Profile object.
*AllApi* | [**GetMsgVpnAclProfileClientConnectException**](docs/AllApi.md#getmsgvpnaclprofileclientconnectexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions/{clientConnectExceptionAddress} | Get a Client Connect Exception object.
*AllApi* | [**GetMsgVpnAclProfileClientConnectExceptions**](docs/AllApi.md#getmsgvpnaclprofileclientconnectexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions | Get a list of Client Connect Exception objects.
*AllApi* | [**GetMsgVpnAclProfilePublishException**](docs/AllApi.md#getmsgvpnaclprofilepublishexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions/{topicSyntax},{publishExceptionTopic} | Get a Publish Topic Exception object.
*AllApi* | [**GetMsgVpnAclProfilePublishExceptions**](docs/AllApi.md#getmsgvpnaclprofilepublishexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions | Get a list of Publish Topic Exception objects.
*AllApi* | [**GetMsgVpnAclProfilePublishTopicException**](docs/AllApi.md#getmsgvpnaclprofilepublishtopicexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions/{publishTopicExceptionSyntax},{publishTopicException} | Get a Publish Topic Exception object.
*AllApi* | [**GetMsgVpnAclProfilePublishTopicExceptions**](docs/AllApi.md#getmsgvpnaclprofilepublishtopicexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions | Get a list of Publish Topic Exception objects.
*AllApi* | [**GetMsgVpnAclProfileSubscribeException**](docs/AllApi.md#getmsgvpnaclprofilesubscribeexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions/{topicSyntax},{subscribeExceptionTopic} | Get a Subscribe Topic Exception object.
*AllApi* | [**GetMsgVpnAclProfileSubscribeExceptions**](docs/AllApi.md#getmsgvpnaclprofilesubscribeexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions | Get a list of Subscribe Topic Exception objects.
*AllApi* | [**GetMsgVpnAclProfileSubscribeShareNameException**](docs/AllApi.md#getmsgvpnaclprofilesubscribesharenameexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions/{subscribeShareNameExceptionSyntax},{subscribeShareNameException} | Get a Subscribe Share Name Exception object.
*AllApi* | [**GetMsgVpnAclProfileSubscribeShareNameExceptions**](docs/AllApi.md#getmsgvpnaclprofilesubscribesharenameexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions | Get a list of Subscribe Share Name Exception objects.
*AllApi* | [**GetMsgVpnAclProfileSubscribeTopicException**](docs/AllApi.md#getmsgvpnaclprofilesubscribetopicexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions/{subscribeTopicExceptionSyntax},{subscribeTopicException} | Get a Subscribe Topic Exception object.
*AllApi* | [**GetMsgVpnAclProfileSubscribeTopicExceptions**](docs/AllApi.md#getmsgvpnaclprofilesubscribetopicexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions | Get a list of Subscribe Topic Exception objects.
*AllApi* | [**GetMsgVpnAclProfiles**](docs/AllApi.md#getmsgvpnaclprofiles) | **GET** /msgVpns/{msgVpnName}/aclProfiles | Get a list of ACL Profile objects.
*AllApi* | [**GetMsgVpnAuthenticationOauthProfile**](docs/AllApi.md#getmsgvpnauthenticationoauthprofile) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Get an OAuth Profile object.
*AllApi* | [**GetMsgVpnAuthenticationOauthProfileClientRequiredClaim**](docs/AllApi.md#getmsgvpnauthenticationoauthprofileclientrequiredclaim) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Get a Required Claim object.
*AllApi* | [**GetMsgVpnAuthenticationOauthProfileClientRequiredClaims**](docs/AllApi.md#getmsgvpnauthenticationoauthprofileclientrequiredclaims) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims | Get a list of Required Claim objects.
*AllApi* | [**GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**](docs/AllApi.md#getmsgvpnauthenticationoauthprofileresourceserverrequiredclaim) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Get a Required Claim object.
*AllApi* | [**GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaims**](docs/AllApi.md#getmsgvpnauthenticationoauthprofileresourceserverrequiredclaims) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Get a list of Required Claim objects.
*AllApi* | [**GetMsgVpnAuthenticationOauthProfiles**](docs/AllApi.md#getmsgvpnauthenticationoauthprofiles) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles | Get a list of OAuth Profile objects.
*AllApi* | [**GetMsgVpnAuthenticationOauthProvider**](docs/AllApi.md#getmsgvpnauthenticationoauthprovider) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Get an OAuth Provider object.
*AllApi* | [**GetMsgVpnAuthenticationOauthProviders**](docs/AllApi.md#getmsgvpnauthenticationoauthproviders) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProviders | Get a list of OAuth Provider objects.
*AllApi* | [**GetMsgVpnAuthorizationGroup**](docs/AllApi.md#getmsgvpnauthorizationgroup) | **GET** /msgVpns/{msgVpnName}/authorizationGroups/{authorizationGroupName} | Get an Authorization Group object.
*AllApi* | [**GetMsgVpnAuthorizationGroups**](docs/AllApi.md#getmsgvpnauthorizationgroups) | **GET** /msgVpns/{msgVpnName}/authorizationGroups | Get a list of Authorization Group objects.
*AllApi* | [**GetMsgVpnBridge**](docs/AllApi.md#getmsgvpnbridge) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Get a Bridge object.
*AllApi* | [**GetMsgVpnBridgeRemoteMsgVpn**](docs/AllApi.md#getmsgvpnbridgeremotemsgvpn) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Get a Remote Message VPN object.
*AllApi* | [**GetMsgVpnBridgeRemoteMsgVpns**](docs/AllApi.md#getmsgvpnbridgeremotemsgvpns) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns | Get a list of Remote Message VPN objects.
*AllApi* | [**GetMsgVpnBridgeRemoteSubscription**](docs/AllApi.md#getmsgvpnbridgeremotesubscription) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions/{remoteSubscriptionTopic} | Get a Remote Subscription object.
*AllApi* | [**GetMsgVpnBridgeRemoteSubscriptions**](docs/AllApi.md#getmsgvpnbridgeremotesubscriptions) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions | Get a list of Remote Subscription objects.
*AllApi* | [**GetMsgVpnBridgeTlsTrustedCommonName**](docs/AllApi.md#getmsgvpnbridgetlstrustedcommonname) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Get a Trusted Common Name object.
*AllApi* | [**GetMsgVpnBridgeTlsTrustedCommonNames**](docs/AllApi.md#getmsgvpnbridgetlstrustedcommonnames) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames | Get a list of Trusted Common Name objects.
*AllApi* | [**GetMsgVpnBridges**](docs/AllApi.md#getmsgvpnbridges) | **GET** /msgVpns/{msgVpnName}/bridges | Get a list of Bridge objects.
*AllApi* | [**GetMsgVpnCertMatchingRule**](docs/AllApi.md#getmsgvpncertmatchingrule) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Get a Certificate Matching Rule object.
*AllApi* | [**GetMsgVpnCertMatchingRuleAttributeFilter**](docs/AllApi.md#getmsgvpncertmatchingruleattributefilter) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Get a Certificate Matching Rule Attribute Filter object.
*AllApi* | [**GetMsgVpnCertMatchingRuleAttributeFilters**](docs/AllApi.md#getmsgvpncertmatchingruleattributefilters) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters | Get a list of Certificate Matching Rule Attribute Filter objects.
*AllApi* | [**GetMsgVpnCertMatchingRuleCondition**](docs/AllApi.md#getmsgvpncertmatchingrulecondition) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions/{source} | Get a Certificate Matching Rule Condition object.
*AllApi* | [**GetMsgVpnCertMatchingRuleConditions**](docs/AllApi.md#getmsgvpncertmatchingruleconditions) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions | Get a list of Certificate Matching Rule Condition objects.
*AllApi* | [**GetMsgVpnCertMatchingRules**](docs/AllApi.md#getmsgvpncertmatchingrules) | **GET** /msgVpns/{msgVpnName}/certMatchingRules | Get a list of Certificate Matching Rule objects.
*AllApi* | [**GetMsgVpnClientProfile**](docs/AllApi.md#getmsgvpnclientprofile) | **GET** /msgVpns/{msgVpnName}/clientProfiles/{clientProfileName} | Get a Client Profile object.
*AllApi* | [**GetMsgVpnClientProfiles**](docs/AllApi.md#getmsgvpnclientprofiles) | **GET** /msgVpns/{msgVpnName}/clientProfiles | Get a list of Client Profile objects.
*AllApi* | [**GetMsgVpnClientUsername**](docs/AllApi.md#getmsgvpnclientusername) | **GET** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername} | Get a Client Username object.
*AllApi* | [**GetMsgVpnClientUsernameAttribute**](docs/AllApi.md#getmsgvpnclientusernameattribute) | **GET** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername}/attributes/{attributeName},{attributeValue} | Get a Client Username Attribute object.
*AllApi* | [**GetMsgVpnClientUsernameAttributes**](docs/AllApi.md#getmsgvpnclientusernameattributes) | **GET** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername}/attributes | Get a list of Client Username Attribute objects.
*AllApi* | [**GetMsgVpnClientUsernames**](docs/AllApi.md#getmsgvpnclientusernames) | **GET** /msgVpns/{msgVpnName}/clientUsernames | Get a list of Client Username objects.
*AllApi* | [**GetMsgVpnDistributedCache**](docs/AllApi.md#getmsgvpndistributedcache) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Get a Distributed Cache object.
*AllApi* | [**GetMsgVpnDistributedCacheCluster**](docs/AllApi.md#getmsgvpndistributedcachecluster) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Get a Cache Cluster object.
*AllApi* | [**GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**](docs/AllApi.md#getmsgvpndistributedcacheclusterglobalcachinghomecluster) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName} | Get a Home Cache Cluster object.
*AllApi* | [**GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**](docs/AllApi.md#getmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes/{topicPrefix} | Get a Topic Prefix object.
*AllApi* | [**GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes**](docs/AllApi.md#getmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefixes) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes | Get a list of Topic Prefix objects.
*AllApi* | [**GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters**](docs/AllApi.md#getmsgvpndistributedcacheclusterglobalcachinghomeclusters) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters | Get a list of Home Cache Cluster objects.
*AllApi* | [**GetMsgVpnDistributedCacheClusterInstance**](docs/AllApi.md#getmsgvpndistributedcacheclusterinstance) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Get a Cache Instance object.
*AllApi* | [**GetMsgVpnDistributedCacheClusterInstances**](docs/AllApi.md#getmsgvpndistributedcacheclusterinstances) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances | Get a list of Cache Instance objects.
*AllApi* | [**GetMsgVpnDistributedCacheClusterTopic**](docs/AllApi.md#getmsgvpndistributedcacheclustertopic) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics/{topic} | Get a Topic object.
*AllApi* | [**GetMsgVpnDistributedCacheClusterTopics**](docs/AllApi.md#getmsgvpndistributedcacheclustertopics) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics | Get a list of Topic objects.
*AllApi* | [**GetMsgVpnDistributedCacheClusters**](docs/AllApi.md#getmsgvpndistributedcacheclusters) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters | Get a list of Cache Cluster objects.
*AllApi* | [**GetMsgVpnDistributedCaches**](docs/AllApi.md#getmsgvpndistributedcaches) | **GET** /msgVpns/{msgVpnName}/distributedCaches | Get a list of Distributed Cache objects.
*AllApi* | [**GetMsgVpnDmrBridge**](docs/AllApi.md#getmsgvpndmrbridge) | **GET** /msgVpns/{msgVpnName}/dmrBridges/{remoteNodeName} | Get a DMR Bridge object.
*AllApi* | [**GetMsgVpnDmrBridges**](docs/AllApi.md#getmsgvpndmrbridges) | **GET** /msgVpns/{msgVpnName}/dmrBridges | Get a list of DMR Bridge objects.
*AllApi* | [**GetMsgVpnJndiConnectionFactories**](docs/AllApi.md#getmsgvpnjndiconnectionfactories) | **GET** /msgVpns/{msgVpnName}/jndiConnectionFactories | Get a list of JNDI Connection Factory objects.
*AllApi* | [**GetMsgVpnJndiConnectionFactory**](docs/AllApi.md#getmsgvpnjndiconnectionfactory) | **GET** /msgVpns/{msgVpnName}/jndiConnectionFactories/{connectionFactoryName} | Get a JNDI Connection Factory object.
*AllApi* | [**GetMsgVpnJndiQueue**](docs/AllApi.md#getmsgvpnjndiqueue) | **GET** /msgVpns/{msgVpnName}/jndiQueues/{queueName} | Get a JNDI Queue object.
*AllApi* | [**GetMsgVpnJndiQueues**](docs/AllApi.md#getmsgvpnjndiqueues) | **GET** /msgVpns/{msgVpnName}/jndiQueues | Get a list of JNDI Queue objects.
*AllApi* | [**GetMsgVpnJndiTopic**](docs/AllApi.md#getmsgvpnjnditopic) | **GET** /msgVpns/{msgVpnName}/jndiTopics/{topicName} | Get a JNDI Topic object.
*AllApi* | [**GetMsgVpnJndiTopics**](docs/AllApi.md#getmsgvpnjnditopics) | **GET** /msgVpns/{msgVpnName}/jndiTopics | Get a list of JNDI Topic objects.
*AllApi* | [**GetMsgVpnMqttRetainCache**](docs/AllApi.md#getmsgvpnmqttretaincache) | **GET** /msgVpns/{msgVpnName}/mqttRetainCaches/{cacheName} | Get an MQTT Retain Cache object.
*AllApi* | [**GetMsgVpnMqttRetainCaches**](docs/AllApi.md#getmsgvpnmqttretaincaches) | **GET** /msgVpns/{msgVpnName}/mqttRetainCaches | Get a list of MQTT Retain Cache objects.
*AllApi* | [**GetMsgVpnMqttSession**](docs/AllApi.md#getmsgvpnmqttsession) | **GET** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter} | Get an MQTT Session object.
*AllApi* | [**GetMsgVpnMqttSessionSubscription**](docs/AllApi.md#getmsgvpnmqttsessionsubscription) | **GET** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions/{subscriptionTopic} | Get a Subscription object.
*AllApi* | [**GetMsgVpnMqttSessionSubscriptions**](docs/AllApi.md#getmsgvpnmqttsessionsubscriptions) | **GET** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions | Get a list of Subscription objects.
*AllApi* | [**GetMsgVpnMqttSessions**](docs/AllApi.md#getmsgvpnmqttsessions) | **GET** /msgVpns/{msgVpnName}/mqttSessions | Get a list of MQTT Session objects.
*AllApi* | [**GetMsgVpnQueue**](docs/AllApi.md#getmsgvpnqueue) | **GET** /msgVpns/{msgVpnName}/queues/{queueName} | Get a Queue object.
*AllApi* | [**GetMsgVpnQueueSubscription**](docs/AllApi.md#getmsgvpnqueuesubscription) | **GET** /msgVpns/{msgVpnName}/queues/{queueName}/subscriptions/{subscriptionTopic} | Get a Queue Subscription object.
*AllApi* | [**GetMsgVpnQueueSubscriptions**](docs/AllApi.md#getmsgvpnqueuesubscriptions) | **GET** /msgVpns/{msgVpnName}/queues/{queueName}/subscriptions | Get a list of Queue Subscription objects.
*AllApi* | [**GetMsgVpnQueueTemplate**](docs/AllApi.md#getmsgvpnqueuetemplate) | **GET** /msgVpns/{msgVpnName}/queueTemplates/{queueTemplateName} | Get a Queue Template object.
*AllApi* | [**GetMsgVpnQueueTemplates**](docs/AllApi.md#getmsgvpnqueuetemplates) | **GET** /msgVpns/{msgVpnName}/queueTemplates | Get a list of Queue Template objects.
*AllApi* | [**GetMsgVpnQueues**](docs/AllApi.md#getmsgvpnqueues) | **GET** /msgVpns/{msgVpnName}/queues | Get a list of Queue objects.
*AllApi* | [**GetMsgVpnReplayLog**](docs/AllApi.md#getmsgvpnreplaylog) | **GET** /msgVpns/{msgVpnName}/replayLogs/{replayLogName} | Get a Replay Log object.
*AllApi* | [**GetMsgVpnReplayLogTopicFilterSubscription**](docs/AllApi.md#getmsgvpnreplaylogtopicfiltersubscription) | **GET** /msgVpns/{msgVpnName}/replayLogs/{replayLogName}/topicFilterSubscriptions/{topicFilterSubscription} | Get a Topic Filter Subscription object.
*AllApi* | [**GetMsgVpnReplayLogTopicFilterSubscriptions**](docs/AllApi.md#getmsgvpnreplaylogtopicfiltersubscriptions) | **GET** /msgVpns/{msgVpnName}/replayLogs/{replayLogName}/topicFilterSubscriptions | Get a list of Topic Filter Subscription objects.
*AllApi* | [**GetMsgVpnReplayLogs**](docs/AllApi.md#getmsgvpnreplaylogs) | **GET** /msgVpns/{msgVpnName}/replayLogs | Get a list of Replay Log objects.
*AllApi* | [**GetMsgVpnReplicatedTopic**](docs/AllApi.md#getmsgvpnreplicatedtopic) | **GET** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Get a Replicated Topic object.
*AllApi* | [**GetMsgVpnReplicatedTopics**](docs/AllApi.md#getmsgvpnreplicatedtopics) | **GET** /msgVpns/{msgVpnName}/replicatedTopics | Get a list of Replicated Topic objects.
*AllApi* | [**GetMsgVpnRestDeliveryPoint**](docs/AllApi.md#getmsgvpnrestdeliverypoint) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Get a REST Delivery Point object.
*AllApi* | [**GetMsgVpnRestDeliveryPointQueueBinding**](docs/AllApi.md#getmsgvpnrestdeliverypointqueuebinding) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Get a Queue Binding object.
*AllApi* | [**GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/AllApi.md#getmsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Get a Protected Request Header object.
*AllApi* | [**GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaders**](docs/AllApi.md#getmsgvpnrestdeliverypointqueuebindingprotectedrequestheaders) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders | Get a list of Protected Request Header objects.
*AllApi* | [**GetMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/AllApi.md#getmsgvpnrestdeliverypointqueuebindingrequestheader) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Get a Request Header object.
*AllApi* | [**GetMsgVpnRestDeliveryPointQueueBindingRequestHeaders**](docs/AllApi.md#getmsgvpnrestdeliverypointqueuebindingrequestheaders) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders | Get a list of Request Header objects.
*AllApi* | [**GetMsgVpnRestDeliveryPointQueueBindings**](docs/AllApi.md#getmsgvpnrestdeliverypointqueuebindings) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings | Get a list of Queue Binding objects.
*AllApi* | [**GetMsgVpnRestDeliveryPointRestConsumer**](docs/AllApi.md#getmsgvpnrestdeliverypointrestconsumer) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Get a REST Consumer object.
*AllApi* | [**GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**](docs/AllApi.md#getmsgvpnrestdeliverypointrestconsumeroauthjwtclaim) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims/{oauthJwtClaimName} | Get a Claim object.
*AllApi* | [**GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaims**](docs/AllApi.md#getmsgvpnrestdeliverypointrestconsumeroauthjwtclaims) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims | Get a list of Claim objects.
*AllApi* | [**GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**](docs/AllApi.md#getmsgvpnrestdeliverypointrestconsumertlstrustedcommonname) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Get a Trusted Common Name object.
*AllApi* | [**GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNames**](docs/AllApi.md#getmsgvpnrestdeliverypointrestconsumertlstrustedcommonnames) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames | Get a list of Trusted Common Name objects.
*AllApi* | [**GetMsgVpnRestDeliveryPointRestConsumers**](docs/AllApi.md#getmsgvpnrestdeliverypointrestconsumers) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers | Get a list of REST Consumer objects.
*AllApi* | [**GetMsgVpnRestDeliveryPoints**](docs/AllApi.md#getmsgvpnrestdeliverypoints) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints | Get a list of REST Delivery Point objects.
*AllApi* | [**GetMsgVpnSequencedTopic**](docs/AllApi.md#getmsgvpnsequencedtopic) | **GET** /msgVpns/{msgVpnName}/sequencedTopics/{sequencedTopic} | Get a Sequenced Topic object.
*AllApi* | [**GetMsgVpnSequencedTopics**](docs/AllApi.md#getmsgvpnsequencedtopics) | **GET** /msgVpns/{msgVpnName}/sequencedTopics | Get a list of Sequenced Topic objects.
*AllApi* | [**GetMsgVpnTelemetryProfile**](docs/AllApi.md#getmsgvpntelemetryprofile) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Get a Telemetry Profile object.
*AllApi* | [**GetMsgVpnTelemetryProfileReceiverAclConnectException**](docs/AllApi.md#getmsgvpntelemetryprofilereceiveraclconnectexception) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions/{receiverAclConnectExceptionAddress} | Get a Receiver ACL Connect Exception object.
*AllApi* | [**GetMsgVpnTelemetryProfileReceiverAclConnectExceptions**](docs/AllApi.md#getmsgvpntelemetryprofilereceiveraclconnectexceptions) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions | Get a list of Receiver ACL Connect Exception objects.
*AllApi* | [**GetMsgVpnTelemetryProfileTraceFilter**](docs/AllApi.md#getmsgvpntelemetryprofiletracefilter) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Get a Trace Filter object.
*AllApi* | [**GetMsgVpnTelemetryProfileTraceFilterSubscription**](docs/AllApi.md#getmsgvpntelemetryprofiletracefiltersubscription) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions/{subscription},{subscriptionSyntax} | Get a Telemetry Trace Filter Subscription object.
*AllApi* | [**GetMsgVpnTelemetryProfileTraceFilterSubscriptions**](docs/AllApi.md#getmsgvpntelemetryprofiletracefiltersubscriptions) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions | Get a list of Telemetry Trace Filter Subscription objects.
*AllApi* | [**GetMsgVpnTelemetryProfileTraceFilters**](docs/AllApi.md#getmsgvpntelemetryprofiletracefilters) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters | Get a list of Trace Filter objects.
*AllApi* | [**GetMsgVpnTelemetryProfiles**](docs/AllApi.md#getmsgvpntelemetryprofiles) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles | Get a list of Telemetry Profile objects.
*AllApi* | [**GetMsgVpnTopicEndpoint**](docs/AllApi.md#getmsgvpntopicendpoint) | **GET** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Get a Topic Endpoint object.
*AllApi* | [**GetMsgVpnTopicEndpointTemplate**](docs/AllApi.md#getmsgvpntopicendpointtemplate) | **GET** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Get a Topic Endpoint Template object.
*AllApi* | [**GetMsgVpnTopicEndpointTemplates**](docs/AllApi.md#getmsgvpntopicendpointtemplates) | **GET** /msgVpns/{msgVpnName}/topicEndpointTemplates | Get a list of Topic Endpoint Template objects.
*AllApi* | [**GetMsgVpnTopicEndpoints**](docs/AllApi.md#getmsgvpntopicendpoints) | **GET** /msgVpns/{msgVpnName}/topicEndpoints | Get a list of Topic Endpoint objects.
*AllApi* | [**GetMsgVpns**](docs/AllApi.md#getmsgvpns) | **GET** /msgVpns | Get a list of Message VPN objects.
*AllApi* | [**GetOauthProfile**](docs/AllApi.md#getoauthprofile) | **GET** /oauthProfiles/{oauthProfileName} | Get an OAuth Profile object.
*AllApi* | [**GetOauthProfileAccessLevelGroup**](docs/AllApi.md#getoauthprofileaccesslevelgroup) | **GET** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName} | Get a Group Access Level object.
*AllApi* | [**GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](docs/AllApi.md#getoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **GET** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName} | Get a Message VPN Access-Level Exception object.
*AllApi* | [**GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions**](docs/AllApi.md#getoauthprofileaccesslevelgroupmsgvpnaccesslevelexceptions) | **GET** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions | Get a list of Message VPN Access-Level Exception objects.
*AllApi* | [**GetOauthProfileAccessLevelGroups**](docs/AllApi.md#getoauthprofileaccesslevelgroups) | **GET** /oauthProfiles/{oauthProfileName}/accessLevelGroups | Get a list of Group Access Level objects.
*AllApi* | [**GetOauthProfileClientAllowedHost**](docs/AllApi.md#getoauthprofileclientallowedhost) | **GET** /oauthProfiles/{oauthProfileName}/clientAllowedHosts/{allowedHost} | Get an Allowed Host Value object.
*AllApi* | [**GetOauthProfileClientAllowedHosts**](docs/AllApi.md#getoauthprofileclientallowedhosts) | **GET** /oauthProfiles/{oauthProfileName}/clientAllowedHosts | Get a list of Allowed Host Value objects.
*AllApi* | [**GetOauthProfileClientAuthorizationParameter**](docs/AllApi.md#getoauthprofileclientauthorizationparameter) | **GET** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName} | Get an Authorization Parameter object.
*AllApi* | [**GetOauthProfileClientAuthorizationParameters**](docs/AllApi.md#getoauthprofileclientauthorizationparameters) | **GET** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters | Get a list of Authorization Parameter objects.
*AllApi* | [**GetOauthProfileClientRequiredClaim**](docs/AllApi.md#getoauthprofileclientrequiredclaim) | **GET** /oauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Get a Required Claim object.
*AllApi* | [**GetOauthProfileClientRequiredClaims**](docs/AllApi.md#getoauthprofileclientrequiredclaims) | **GET** /oauthProfiles/{oauthProfileName}/clientRequiredClaims | Get a list of Required Claim objects.
*AllApi* | [**GetOauthProfileDefaultMsgVpnAccessLevelException**](docs/AllApi.md#getoauthprofiledefaultmsgvpnaccesslevelexception) | **GET** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName} | Get a Message VPN Access-Level Exception object.
*AllApi* | [**GetOauthProfileDefaultMsgVpnAccessLevelExceptions**](docs/AllApi.md#getoauthprofiledefaultmsgvpnaccesslevelexceptions) | **GET** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions | Get a list of Message VPN Access-Level Exception objects.
*AllApi* | [**GetOauthProfileResourceServerRequiredClaim**](docs/AllApi.md#getoauthprofileresourceserverrequiredclaim) | **GET** /oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Get a Required Claim object.
*AllApi* | [**GetOauthProfileResourceServerRequiredClaims**](docs/AllApi.md#getoauthprofileresourceserverrequiredclaims) | **GET** /oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Get a list of Required Claim objects.
*AllApi* | [**GetOauthProfiles**](docs/AllApi.md#getoauthprofiles) | **GET** /oauthProfiles | Get a list of OAuth Profile objects.
*AllApi* | [**GetSystemInformation**](docs/AllApi.md#getsysteminformation) | **GET** /systemInformation | Get a System Information object.
*AllApi* | [**GetVirtualHostname**](docs/AllApi.md#getvirtualhostname) | **GET** /virtualHostnames/{virtualHostname} | Get a Virtual Hostname object.
*AllApi* | [**GetVirtualHostnames**](docs/AllApi.md#getvirtualhostnames) | **GET** /virtualHostnames | Get a list of Virtual Hostname objects.
*AllApi* | [**ReplaceCertAuthority**](docs/AllApi.md#replacecertauthority) | **PUT** /certAuthorities/{certAuthorityName} | Replace a Certificate Authority object.
*AllApi* | [**ReplaceClientCertAuthority**](docs/AllApi.md#replaceclientcertauthority) | **PUT** /clientCertAuthorities/{certAuthorityName} | Replace a Client Certificate Authority object.
*AllApi* | [**ReplaceDmrCluster**](docs/AllApi.md#replacedmrcluster) | **PUT** /dmrClusters/{dmrClusterName} | Replace a Cluster object.
*AllApi* | [**ReplaceDmrClusterCertMatchingRule**](docs/AllApi.md#replacedmrclustercertmatchingrule) | **PUT** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName} | Replace a Certificate Matching Rule object.
*AllApi* | [**ReplaceDmrClusterCertMatchingRuleAttributeFilter**](docs/AllApi.md#replacedmrclustercertmatchingruleattributefilter) | **PUT** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Replace a Certificate Matching Rule Attribute Filter object.
*AllApi* | [**ReplaceDmrClusterLink**](docs/AllApi.md#replacedmrclusterlink) | **PUT** /dmrClusters/{dmrClusterName}/links/{remoteNodeName} | Replace a Link object.
*AllApi* | [**ReplaceDomainCertAuthority**](docs/AllApi.md#replacedomaincertauthority) | **PUT** /domainCertAuthorities/{certAuthorityName} | Replace a Domain Certificate Authority object.
*AllApi* | [**ReplaceMsgVpn**](docs/AllApi.md#replacemsgvpn) | **PUT** /msgVpns/{msgVpnName} | Replace a Message VPN object.
*AllApi* | [**ReplaceMsgVpnAclProfile**](docs/AllApi.md#replacemsgvpnaclprofile) | **PUT** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName} | Replace an ACL Profile object.
*AllApi* | [**ReplaceMsgVpnAuthenticationOauthProfile**](docs/AllApi.md#replacemsgvpnauthenticationoauthprofile) | **PUT** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Replace an OAuth Profile object.
*AllApi* | [**ReplaceMsgVpnAuthenticationOauthProvider**](docs/AllApi.md#replacemsgvpnauthenticationoauthprovider) | **PUT** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Replace an OAuth Provider object.
*AllApi* | [**ReplaceMsgVpnAuthorizationGroup**](docs/AllApi.md#replacemsgvpnauthorizationgroup) | **PUT** /msgVpns/{msgVpnName}/authorizationGroups/{authorizationGroupName} | Replace an Authorization Group object.
*AllApi* | [**ReplaceMsgVpnBridge**](docs/AllApi.md#replacemsgvpnbridge) | **PUT** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Replace a Bridge object.
*AllApi* | [**ReplaceMsgVpnBridgeRemoteMsgVpn**](docs/AllApi.md#replacemsgvpnbridgeremotemsgvpn) | **PUT** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Replace a Remote Message VPN object.
*AllApi* | [**ReplaceMsgVpnCertMatchingRule**](docs/AllApi.md#replacemsgvpncertmatchingrule) | **PUT** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Replace a Certificate Matching Rule object.
*AllApi* | [**ReplaceMsgVpnCertMatchingRuleAttributeFilter**](docs/AllApi.md#replacemsgvpncertmatchingruleattributefilter) | **PUT** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Replace a Certificate Matching Rule Attribute Filter object.
*AllApi* | [**ReplaceMsgVpnClientProfile**](docs/AllApi.md#replacemsgvpnclientprofile) | **PUT** /msgVpns/{msgVpnName}/clientProfiles/{clientProfileName} | Replace a Client Profile object.
*AllApi* | [**ReplaceMsgVpnClientUsername**](docs/AllApi.md#replacemsgvpnclientusername) | **PUT** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername} | Replace a Client Username object.
*AllApi* | [**ReplaceMsgVpnDistributedCache**](docs/AllApi.md#replacemsgvpndistributedcache) | **PUT** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Replace a Distributed Cache object.
*AllApi* | [**ReplaceMsgVpnDistributedCacheCluster**](docs/AllApi.md#replacemsgvpndistributedcachecluster) | **PUT** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Replace a Cache Cluster object.
*AllApi* | [**ReplaceMsgVpnDistributedCacheClusterInstance**](docs/AllApi.md#replacemsgvpndistributedcacheclusterinstance) | **PUT** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Replace a Cache Instance object.
*AllApi* | [**ReplaceMsgVpnDmrBridge**](docs/AllApi.md#replacemsgvpndmrbridge) | **PUT** /msgVpns/{msgVpnName}/dmrBridges/{remoteNodeName} | Replace a DMR Bridge object.
*AllApi* | [**ReplaceMsgVpnJndiConnectionFactory**](docs/AllApi.md#replacemsgvpnjndiconnectionfactory) | **PUT** /msgVpns/{msgVpnName}/jndiConnectionFactories/{connectionFactoryName} | Replace a JNDI Connection Factory object.
*AllApi* | [**ReplaceMsgVpnJndiQueue**](docs/AllApi.md#replacemsgvpnjndiqueue) | **PUT** /msgVpns/{msgVpnName}/jndiQueues/{queueName} | Replace a JNDI Queue object.
*AllApi* | [**ReplaceMsgVpnJndiTopic**](docs/AllApi.md#replacemsgvpnjnditopic) | **PUT** /msgVpns/{msgVpnName}/jndiTopics/{topicName} | Replace a JNDI Topic object.
*AllApi* | [**ReplaceMsgVpnMqttRetainCache**](docs/AllApi.md#replacemsgvpnmqttretaincache) | **PUT** /msgVpns/{msgVpnName}/mqttRetainCaches/{cacheName} | Replace an MQTT Retain Cache object.
*AllApi* | [**ReplaceMsgVpnMqttSession**](docs/AllApi.md#replacemsgvpnmqttsession) | **PUT** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter} | Replace an MQTT Session object.
*AllApi* | [**ReplaceMsgVpnMqttSessionSubscription**](docs/AllApi.md#replacemsgvpnmqttsessionsubscription) | **PUT** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions/{subscriptionTopic} | Replace a Subscription object.
*AllApi* | [**ReplaceMsgVpnQueue**](docs/AllApi.md#replacemsgvpnqueue) | **PUT** /msgVpns/{msgVpnName}/queues/{queueName} | Replace a Queue object.
*AllApi* | [**ReplaceMsgVpnQueueTemplate**](docs/AllApi.md#replacemsgvpnqueuetemplate) | **PUT** /msgVpns/{msgVpnName}/queueTemplates/{queueTemplateName} | Replace a Queue Template object.
*AllApi* | [**ReplaceMsgVpnReplayLog**](docs/AllApi.md#replacemsgvpnreplaylog) | **PUT** /msgVpns/{msgVpnName}/replayLogs/{replayLogName} | Replace a Replay Log object.
*AllApi* | [**ReplaceMsgVpnReplicatedTopic**](docs/AllApi.md#replacemsgvpnreplicatedtopic) | **PUT** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Replace a Replicated Topic object.
*AllApi* | [**ReplaceMsgVpnRestDeliveryPoint**](docs/AllApi.md#replacemsgvpnrestdeliverypoint) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Replace a REST Delivery Point object.
*AllApi* | [**ReplaceMsgVpnRestDeliveryPointQueueBinding**](docs/AllApi.md#replacemsgvpnrestdeliverypointqueuebinding) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Replace a Queue Binding object.
*AllApi* | [**ReplaceMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/AllApi.md#replacemsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Replace a Protected Request Header object.
*AllApi* | [**ReplaceMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/AllApi.md#replacemsgvpnrestdeliverypointqueuebindingrequestheader) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Replace a Request Header object.
*AllApi* | [**ReplaceMsgVpnRestDeliveryPointRestConsumer**](docs/AllApi.md#replacemsgvpnrestdeliverypointrestconsumer) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Replace a REST Consumer object.
*AllApi* | [**ReplaceMsgVpnTelemetryProfile**](docs/AllApi.md#replacemsgvpntelemetryprofile) | **PUT** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Replace a Telemetry Profile object.
*AllApi* | [**ReplaceMsgVpnTelemetryProfileTraceFilter**](docs/AllApi.md#replacemsgvpntelemetryprofiletracefilter) | **PUT** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Replace a Trace Filter object.
*AllApi* | [**ReplaceMsgVpnTopicEndpoint**](docs/AllApi.md#replacemsgvpntopicendpoint) | **PUT** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Replace a Topic Endpoint object.
*AllApi* | [**ReplaceMsgVpnTopicEndpointTemplate**](docs/AllApi.md#replacemsgvpntopicendpointtemplate) | **PUT** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Replace a Topic Endpoint Template object.
*AllApi* | [**ReplaceOauthProfile**](docs/AllApi.md#replaceoauthprofile) | **PUT** /oauthProfiles/{oauthProfileName} | Replace an OAuth Profile object.
*AllApi* | [**ReplaceOauthProfileAccessLevelGroup**](docs/AllApi.md#replaceoauthprofileaccesslevelgroup) | **PUT** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName} | Replace a Group Access Level object.
*AllApi* | [**ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](docs/AllApi.md#replaceoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **PUT** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName} | Replace a Message VPN Access-Level Exception object.
*AllApi* | [**ReplaceOauthProfileClientAuthorizationParameter**](docs/AllApi.md#replaceoauthprofileclientauthorizationparameter) | **PUT** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName} | Replace an Authorization Parameter object.
*AllApi* | [**ReplaceOauthProfileDefaultMsgVpnAccessLevelException**](docs/AllApi.md#replaceoauthprofiledefaultmsgvpnaccesslevelexception) | **PUT** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName} | Replace a Message VPN Access-Level Exception object.
*AllApi* | [**ReplaceVirtualHostname**](docs/AllApi.md#replacevirtualhostname) | **PUT** /virtualHostnames/{virtualHostname} | Replace a Virtual Hostname object.
*AllApi* | [**UpdateBroker**](docs/AllApi.md#updatebroker) | **PATCH** / | Update a Broker object.
*AllApi* | [**UpdateCertAuthority**](docs/AllApi.md#updatecertauthority) | **PATCH** /certAuthorities/{certAuthorityName} | Update a Certificate Authority object.
*AllApi* | [**UpdateClientCertAuthority**](docs/AllApi.md#updateclientcertauthority) | **PATCH** /clientCertAuthorities/{certAuthorityName} | Update a Client Certificate Authority object.
*AllApi* | [**UpdateDmrCluster**](docs/AllApi.md#updatedmrcluster) | **PATCH** /dmrClusters/{dmrClusterName} | Update a Cluster object.
*AllApi* | [**UpdateDmrClusterCertMatchingRule**](docs/AllApi.md#updatedmrclustercertmatchingrule) | **PATCH** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName} | Update a Certificate Matching Rule object.
*AllApi* | [**UpdateDmrClusterCertMatchingRuleAttributeFilter**](docs/AllApi.md#updatedmrclustercertmatchingruleattributefilter) | **PATCH** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Update a Certificate Matching Rule Attribute Filter object.
*AllApi* | [**UpdateDmrClusterLink**](docs/AllApi.md#updatedmrclusterlink) | **PATCH** /dmrClusters/{dmrClusterName}/links/{remoteNodeName} | Update a Link object.
*AllApi* | [**UpdateDomainCertAuthority**](docs/AllApi.md#updatedomaincertauthority) | **PATCH** /domainCertAuthorities/{certAuthorityName} | Update a Domain Certificate Authority object.
*AllApi* | [**UpdateMsgVpn**](docs/AllApi.md#updatemsgvpn) | **PATCH** /msgVpns/{msgVpnName} | Update a Message VPN object.
*AllApi* | [**UpdateMsgVpnAclProfile**](docs/AllApi.md#updatemsgvpnaclprofile) | **PATCH** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName} | Update an ACL Profile object.
*AllApi* | [**UpdateMsgVpnAuthenticationOauthProfile**](docs/AllApi.md#updatemsgvpnauthenticationoauthprofile) | **PATCH** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Update an OAuth Profile object.
*AllApi* | [**UpdateMsgVpnAuthenticationOauthProvider**](docs/AllApi.md#updatemsgvpnauthenticationoauthprovider) | **PATCH** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Update an OAuth Provider object.
*AllApi* | [**UpdateMsgVpnAuthorizationGroup**](docs/AllApi.md#updatemsgvpnauthorizationgroup) | **PATCH** /msgVpns/{msgVpnName}/authorizationGroups/{authorizationGroupName} | Update an Authorization Group object.
*AllApi* | [**UpdateMsgVpnBridge**](docs/AllApi.md#updatemsgvpnbridge) | **PATCH** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Update a Bridge object.
*AllApi* | [**UpdateMsgVpnBridgeRemoteMsgVpn**](docs/AllApi.md#updatemsgvpnbridgeremotemsgvpn) | **PATCH** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Update a Remote Message VPN object.
*AllApi* | [**UpdateMsgVpnCertMatchingRule**](docs/AllApi.md#updatemsgvpncertmatchingrule) | **PATCH** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Update a Certificate Matching Rule object.
*AllApi* | [**UpdateMsgVpnCertMatchingRuleAttributeFilter**](docs/AllApi.md#updatemsgvpncertmatchingruleattributefilter) | **PATCH** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Update a Certificate Matching Rule Attribute Filter object.
*AllApi* | [**UpdateMsgVpnClientProfile**](docs/AllApi.md#updatemsgvpnclientprofile) | **PATCH** /msgVpns/{msgVpnName}/clientProfiles/{clientProfileName} | Update a Client Profile object.
*AllApi* | [**UpdateMsgVpnClientUsername**](docs/AllApi.md#updatemsgvpnclientusername) | **PATCH** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername} | Update a Client Username object.
*AllApi* | [**UpdateMsgVpnDistributedCache**](docs/AllApi.md#updatemsgvpndistributedcache) | **PATCH** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Update a Distributed Cache object.
*AllApi* | [**UpdateMsgVpnDistributedCacheCluster**](docs/AllApi.md#updatemsgvpndistributedcachecluster) | **PATCH** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Update a Cache Cluster object.
*AllApi* | [**UpdateMsgVpnDistributedCacheClusterInstance**](docs/AllApi.md#updatemsgvpndistributedcacheclusterinstance) | **PATCH** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Update a Cache Instance object.
*AllApi* | [**UpdateMsgVpnDmrBridge**](docs/AllApi.md#updatemsgvpndmrbridge) | **PATCH** /msgVpns/{msgVpnName}/dmrBridges/{remoteNodeName} | Update a DMR Bridge object.
*AllApi* | [**UpdateMsgVpnJndiConnectionFactory**](docs/AllApi.md#updatemsgvpnjndiconnectionfactory) | **PATCH** /msgVpns/{msgVpnName}/jndiConnectionFactories/{connectionFactoryName} | Update a JNDI Connection Factory object.
*AllApi* | [**UpdateMsgVpnJndiQueue**](docs/AllApi.md#updatemsgvpnjndiqueue) | **PATCH** /msgVpns/{msgVpnName}/jndiQueues/{queueName} | Update a JNDI Queue object.
*AllApi* | [**UpdateMsgVpnJndiTopic**](docs/AllApi.md#updatemsgvpnjnditopic) | **PATCH** /msgVpns/{msgVpnName}/jndiTopics/{topicName} | Update a JNDI Topic object.
*AllApi* | [**UpdateMsgVpnMqttRetainCache**](docs/AllApi.md#updatemsgvpnmqttretaincache) | **PATCH** /msgVpns/{msgVpnName}/mqttRetainCaches/{cacheName} | Update an MQTT Retain Cache object.
*AllApi* | [**UpdateMsgVpnMqttSession**](docs/AllApi.md#updatemsgvpnmqttsession) | **PATCH** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter} | Update an MQTT Session object.
*AllApi* | [**UpdateMsgVpnMqttSessionSubscription**](docs/AllApi.md#updatemsgvpnmqttsessionsubscription) | **PATCH** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions/{subscriptionTopic} | Update a Subscription object.
*AllApi* | [**UpdateMsgVpnQueue**](docs/AllApi.md#updatemsgvpnqueue) | **PATCH** /msgVpns/{msgVpnName}/queues/{queueName} | Update a Queue object.
*AllApi* | [**UpdateMsgVpnQueueTemplate**](docs/AllApi.md#updatemsgvpnqueuetemplate) | **PATCH** /msgVpns/{msgVpnName}/queueTemplates/{queueTemplateName} | Update a Queue Template object.
*AllApi* | [**UpdateMsgVpnReplayLog**](docs/AllApi.md#updatemsgvpnreplaylog) | **PATCH** /msgVpns/{msgVpnName}/replayLogs/{replayLogName} | Update a Replay Log object.
*AllApi* | [**UpdateMsgVpnReplicatedTopic**](docs/AllApi.md#updatemsgvpnreplicatedtopic) | **PATCH** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Update a Replicated Topic object.
*AllApi* | [**UpdateMsgVpnRestDeliveryPoint**](docs/AllApi.md#updatemsgvpnrestdeliverypoint) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Update a REST Delivery Point object.
*AllApi* | [**UpdateMsgVpnRestDeliveryPointQueueBinding**](docs/AllApi.md#updatemsgvpnrestdeliverypointqueuebinding) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Update a Queue Binding object.
*AllApi* | [**UpdateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/AllApi.md#updatemsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Update a Protected Request Header object.
*AllApi* | [**UpdateMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/AllApi.md#updatemsgvpnrestdeliverypointqueuebindingrequestheader) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Update a Request Header object.
*AllApi* | [**UpdateMsgVpnRestDeliveryPointRestConsumer**](docs/AllApi.md#updatemsgvpnrestdeliverypointrestconsumer) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Update a REST Consumer object.
*AllApi* | [**UpdateMsgVpnTelemetryProfile**](docs/AllApi.md#updatemsgvpntelemetryprofile) | **PATCH** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Update a Telemetry Profile object.
*AllApi* | [**UpdateMsgVpnTelemetryProfileTraceFilter**](docs/AllApi.md#updatemsgvpntelemetryprofiletracefilter) | **PATCH** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Update a Trace Filter object.
*AllApi* | [**UpdateMsgVpnTopicEndpoint**](docs/AllApi.md#updatemsgvpntopicendpoint) | **PATCH** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Update a Topic Endpoint object.
*AllApi* | [**UpdateMsgVpnTopicEndpointTemplate**](docs/AllApi.md#updatemsgvpntopicendpointtemplate) | **PATCH** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Update a Topic Endpoint Template object.
*AllApi* | [**UpdateOauthProfile**](docs/AllApi.md#updateoauthprofile) | **PATCH** /oauthProfiles/{oauthProfileName} | Update an OAuth Profile object.
*AllApi* | [**UpdateOauthProfileAccessLevelGroup**](docs/AllApi.md#updateoauthprofileaccesslevelgroup) | **PATCH** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName} | Update a Group Access Level object.
*AllApi* | [**UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](docs/AllApi.md#updateoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **PATCH** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName} | Update a Message VPN Access-Level Exception object.
*AllApi* | [**UpdateOauthProfileClientAuthorizationParameter**](docs/AllApi.md#updateoauthprofileclientauthorizationparameter) | **PATCH** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName} | Update an Authorization Parameter object.
*AllApi* | [**UpdateOauthProfileDefaultMsgVpnAccessLevelException**](docs/AllApi.md#updateoauthprofiledefaultmsgvpnaccesslevelexception) | **PATCH** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName} | Update a Message VPN Access-Level Exception object.
*AllApi* | [**UpdateVirtualHostname**](docs/AllApi.md#updatevirtualhostname) | **PATCH** /virtualHostnames/{virtualHostname} | Update a Virtual Hostname object.
*AuthenticationOauthProfileApi* | [**CreateMsgVpnAuthenticationOauthProfile**](docs/AuthenticationOauthProfileApi.md#createmsgvpnauthenticationoauthprofile) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProfiles | Create an OAuth Profile object.
*AuthenticationOauthProfileApi* | [**CreateMsgVpnAuthenticationOauthProfileClientRequiredClaim**](docs/AuthenticationOauthProfileApi.md#createmsgvpnauthenticationoauthprofileclientrequiredclaim) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims | Create a Required Claim object.
*AuthenticationOauthProfileApi* | [**CreateMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**](docs/AuthenticationOauthProfileApi.md#createmsgvpnauthenticationoauthprofileresourceserverrequiredclaim) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Create a Required Claim object.
*AuthenticationOauthProfileApi* | [**DeleteMsgVpnAuthenticationOauthProfile**](docs/AuthenticationOauthProfileApi.md#deletemsgvpnauthenticationoauthprofile) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Delete an OAuth Profile object.
*AuthenticationOauthProfileApi* | [**DeleteMsgVpnAuthenticationOauthProfileClientRequiredClaim**](docs/AuthenticationOauthProfileApi.md#deletemsgvpnauthenticationoauthprofileclientrequiredclaim) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Delete a Required Claim object.
*AuthenticationOauthProfileApi* | [**DeleteMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**](docs/AuthenticationOauthProfileApi.md#deletemsgvpnauthenticationoauthprofileresourceserverrequiredclaim) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Delete a Required Claim object.
*AuthenticationOauthProfileApi* | [**GetMsgVpnAuthenticationOauthProfile**](docs/AuthenticationOauthProfileApi.md#getmsgvpnauthenticationoauthprofile) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Get an OAuth Profile object.
*AuthenticationOauthProfileApi* | [**GetMsgVpnAuthenticationOauthProfileClientRequiredClaim**](docs/AuthenticationOauthProfileApi.md#getmsgvpnauthenticationoauthprofileclientrequiredclaim) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Get a Required Claim object.
*AuthenticationOauthProfileApi* | [**GetMsgVpnAuthenticationOauthProfileClientRequiredClaims**](docs/AuthenticationOauthProfileApi.md#getmsgvpnauthenticationoauthprofileclientrequiredclaims) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims | Get a list of Required Claim objects.
*AuthenticationOauthProfileApi* | [**GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**](docs/AuthenticationOauthProfileApi.md#getmsgvpnauthenticationoauthprofileresourceserverrequiredclaim) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Get a Required Claim object.
*AuthenticationOauthProfileApi* | [**GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaims**](docs/AuthenticationOauthProfileApi.md#getmsgvpnauthenticationoauthprofileresourceserverrequiredclaims) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Get a list of Required Claim objects.
*AuthenticationOauthProfileApi* | [**GetMsgVpnAuthenticationOauthProfiles**](docs/AuthenticationOauthProfileApi.md#getmsgvpnauthenticationoauthprofiles) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles | Get a list of OAuth Profile objects.
*AuthenticationOauthProfileApi* | [**ReplaceMsgVpnAuthenticationOauthProfile**](docs/AuthenticationOauthProfileApi.md#replacemsgvpnauthenticationoauthprofile) | **PUT** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Replace an OAuth Profile object.
*AuthenticationOauthProfileApi* | [**UpdateMsgVpnAuthenticationOauthProfile**](docs/AuthenticationOauthProfileApi.md#updatemsgvpnauthenticationoauthprofile) | **PATCH** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Update an OAuth Profile object.
*AuthenticationOauthProviderApi* | [**CreateMsgVpnAuthenticationOauthProvider**](docs/AuthenticationOauthProviderApi.md#createmsgvpnauthenticationoauthprovider) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProviders | Create an OAuth Provider object.
*AuthenticationOauthProviderApi* | [**DeleteMsgVpnAuthenticationOauthProvider**](docs/AuthenticationOauthProviderApi.md#deletemsgvpnauthenticationoauthprovider) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Delete an OAuth Provider object.
*AuthenticationOauthProviderApi* | [**GetMsgVpnAuthenticationOauthProvider**](docs/AuthenticationOauthProviderApi.md#getmsgvpnauthenticationoauthprovider) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Get an OAuth Provider object.
*AuthenticationOauthProviderApi* | [**GetMsgVpnAuthenticationOauthProviders**](docs/AuthenticationOauthProviderApi.md#getmsgvpnauthenticationoauthproviders) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProviders | Get a list of OAuth Provider objects.
*AuthenticationOauthProviderApi* | [**ReplaceMsgVpnAuthenticationOauthProvider**](docs/AuthenticationOauthProviderApi.md#replacemsgvpnauthenticationoauthprovider) | **PUT** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Replace an OAuth Provider object.
*AuthenticationOauthProviderApi* | [**UpdateMsgVpnAuthenticationOauthProvider**](docs/AuthenticationOauthProviderApi.md#updatemsgvpnauthenticationoauthprovider) | **PATCH** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Update an OAuth Provider object.
*AuthorizationGroupApi* | [**CreateMsgVpnAuthorizationGroup**](docs/AuthorizationGroupApi.md#createmsgvpnauthorizationgroup) | **POST** /msgVpns/{msgVpnName}/authorizationGroups | Create an Authorization Group object.
*AuthorizationGroupApi* | [**DeleteMsgVpnAuthorizationGroup**](docs/AuthorizationGroupApi.md#deletemsgvpnauthorizationgroup) | **DELETE** /msgVpns/{msgVpnName}/authorizationGroups/{authorizationGroupName} | Delete an Authorization Group object.
*AuthorizationGroupApi* | [**GetMsgVpnAuthorizationGroup**](docs/AuthorizationGroupApi.md#getmsgvpnauthorizationgroup) | **GET** /msgVpns/{msgVpnName}/authorizationGroups/{authorizationGroupName} | Get an Authorization Group object.
*AuthorizationGroupApi* | [**GetMsgVpnAuthorizationGroups**](docs/AuthorizationGroupApi.md#getmsgvpnauthorizationgroups) | **GET** /msgVpns/{msgVpnName}/authorizationGroups | Get a list of Authorization Group objects.
*AuthorizationGroupApi* | [**ReplaceMsgVpnAuthorizationGroup**](docs/AuthorizationGroupApi.md#replacemsgvpnauthorizationgroup) | **PUT** /msgVpns/{msgVpnName}/authorizationGroups/{authorizationGroupName} | Replace an Authorization Group object.
*AuthorizationGroupApi* | [**UpdateMsgVpnAuthorizationGroup**](docs/AuthorizationGroupApi.md#updatemsgvpnauthorizationgroup) | **PATCH** /msgVpns/{msgVpnName}/authorizationGroups/{authorizationGroupName} | Update an Authorization Group object.
*BridgeApi* | [**CreateMsgVpnBridge**](docs/BridgeApi.md#createmsgvpnbridge) | **POST** /msgVpns/{msgVpnName}/bridges | Create a Bridge object.
*BridgeApi* | [**CreateMsgVpnBridgeRemoteMsgVpn**](docs/BridgeApi.md#createmsgvpnbridgeremotemsgvpn) | **POST** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns | Create a Remote Message VPN object.
*BridgeApi* | [**CreateMsgVpnBridgeRemoteSubscription**](docs/BridgeApi.md#createmsgvpnbridgeremotesubscription) | **POST** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions | Create a Remote Subscription object.
*BridgeApi* | [**CreateMsgVpnBridgeTlsTrustedCommonName**](docs/BridgeApi.md#createmsgvpnbridgetlstrustedcommonname) | **POST** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames | Create a Trusted Common Name object.
*BridgeApi* | [**DeleteMsgVpnBridge**](docs/BridgeApi.md#deletemsgvpnbridge) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Delete a Bridge object.
*BridgeApi* | [**DeleteMsgVpnBridgeRemoteMsgVpn**](docs/BridgeApi.md#deletemsgvpnbridgeremotemsgvpn) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Delete a Remote Message VPN object.
*BridgeApi* | [**DeleteMsgVpnBridgeRemoteSubscription**](docs/BridgeApi.md#deletemsgvpnbridgeremotesubscription) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions/{remoteSubscriptionTopic} | Delete a Remote Subscription object.
*BridgeApi* | [**DeleteMsgVpnBridgeTlsTrustedCommonName**](docs/BridgeApi.md#deletemsgvpnbridgetlstrustedcommonname) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Delete a Trusted Common Name object.
*BridgeApi* | [**GetMsgVpnBridge**](docs/BridgeApi.md#getmsgvpnbridge) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Get a Bridge object.
*BridgeApi* | [**GetMsgVpnBridgeRemoteMsgVpn**](docs/BridgeApi.md#getmsgvpnbridgeremotemsgvpn) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Get a Remote Message VPN object.
*BridgeApi* | [**GetMsgVpnBridgeRemoteMsgVpns**](docs/BridgeApi.md#getmsgvpnbridgeremotemsgvpns) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns | Get a list of Remote Message VPN objects.
*BridgeApi* | [**GetMsgVpnBridgeRemoteSubscription**](docs/BridgeApi.md#getmsgvpnbridgeremotesubscription) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions/{remoteSubscriptionTopic} | Get a Remote Subscription object.
*BridgeApi* | [**GetMsgVpnBridgeRemoteSubscriptions**](docs/BridgeApi.md#getmsgvpnbridgeremotesubscriptions) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions | Get a list of Remote Subscription objects.
*BridgeApi* | [**GetMsgVpnBridgeTlsTrustedCommonName**](docs/BridgeApi.md#getmsgvpnbridgetlstrustedcommonname) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Get a Trusted Common Name object.
*BridgeApi* | [**GetMsgVpnBridgeTlsTrustedCommonNames**](docs/BridgeApi.md#getmsgvpnbridgetlstrustedcommonnames) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames | Get a list of Trusted Common Name objects.
*BridgeApi* | [**GetMsgVpnBridges**](docs/BridgeApi.md#getmsgvpnbridges) | **GET** /msgVpns/{msgVpnName}/bridges | Get a list of Bridge objects.
*BridgeApi* | [**ReplaceMsgVpnBridge**](docs/BridgeApi.md#replacemsgvpnbridge) | **PUT** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Replace a Bridge object.
*BridgeApi* | [**ReplaceMsgVpnBridgeRemoteMsgVpn**](docs/BridgeApi.md#replacemsgvpnbridgeremotemsgvpn) | **PUT** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Replace a Remote Message VPN object.
*BridgeApi* | [**UpdateMsgVpnBridge**](docs/BridgeApi.md#updatemsgvpnbridge) | **PATCH** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Update a Bridge object.
*BridgeApi* | [**UpdateMsgVpnBridgeRemoteMsgVpn**](docs/BridgeApi.md#updatemsgvpnbridgeremotemsgvpn) | **PATCH** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Update a Remote Message VPN object.
*CertAuthorityApi* | [**CreateCertAuthority**](docs/CertAuthorityApi.md#createcertauthority) | **POST** /certAuthorities | Create a Certificate Authority object.
*CertAuthorityApi* | [**CreateCertAuthorityOcspTlsTrustedCommonName**](docs/CertAuthorityApi.md#createcertauthorityocsptlstrustedcommonname) | **POST** /certAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames | Create an OCSP Responder Trusted Common Name object.
*CertAuthorityApi* | [**DeleteCertAuthority**](docs/CertAuthorityApi.md#deletecertauthority) | **DELETE** /certAuthorities/{certAuthorityName} | Delete a Certificate Authority object.
*CertAuthorityApi* | [**DeleteCertAuthorityOcspTlsTrustedCommonName**](docs/CertAuthorityApi.md#deletecertauthorityocsptlstrustedcommonname) | **DELETE** /certAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames/{ocspTlsTrustedCommonName} | Delete an OCSP Responder Trusted Common Name object.
*CertAuthorityApi* | [**GetCertAuthorities**](docs/CertAuthorityApi.md#getcertauthorities) | **GET** /certAuthorities | Get a list of Certificate Authority objects.
*CertAuthorityApi* | [**GetCertAuthority**](docs/CertAuthorityApi.md#getcertauthority) | **GET** /certAuthorities/{certAuthorityName} | Get a Certificate Authority object.
*CertAuthorityApi* | [**GetCertAuthorityOcspTlsTrustedCommonName**](docs/CertAuthorityApi.md#getcertauthorityocsptlstrustedcommonname) | **GET** /certAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames/{ocspTlsTrustedCommonName} | Get an OCSP Responder Trusted Common Name object.
*CertAuthorityApi* | [**GetCertAuthorityOcspTlsTrustedCommonNames**](docs/CertAuthorityApi.md#getcertauthorityocsptlstrustedcommonnames) | **GET** /certAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames | Get a list of OCSP Responder Trusted Common Name objects.
*CertAuthorityApi* | [**ReplaceCertAuthority**](docs/CertAuthorityApi.md#replacecertauthority) | **PUT** /certAuthorities/{certAuthorityName} | Replace a Certificate Authority object.
*CertAuthorityApi* | [**UpdateCertAuthority**](docs/CertAuthorityApi.md#updatecertauthority) | **PATCH** /certAuthorities/{certAuthorityName} | Update a Certificate Authority object.
*CertMatchingRuleApi* | [**CreateMsgVpnCertMatchingRule**](docs/CertMatchingRuleApi.md#createmsgvpncertmatchingrule) | **POST** /msgVpns/{msgVpnName}/certMatchingRules | Create a Certificate Matching Rule object.
*CertMatchingRuleApi* | [**CreateMsgVpnCertMatchingRuleAttributeFilter**](docs/CertMatchingRuleApi.md#createmsgvpncertmatchingruleattributefilter) | **POST** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters | Create a Certificate Matching Rule Attribute Filter object.
*CertMatchingRuleApi* | [**CreateMsgVpnCertMatchingRuleCondition**](docs/CertMatchingRuleApi.md#createmsgvpncertmatchingrulecondition) | **POST** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions | Create a Certificate Matching Rule Condition object.
*CertMatchingRuleApi* | [**DeleteMsgVpnCertMatchingRule**](docs/CertMatchingRuleApi.md#deletemsgvpncertmatchingrule) | **DELETE** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Delete a Certificate Matching Rule object.
*CertMatchingRuleApi* | [**DeleteMsgVpnCertMatchingRuleAttributeFilter**](docs/CertMatchingRuleApi.md#deletemsgvpncertmatchingruleattributefilter) | **DELETE** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Delete a Certificate Matching Rule Attribute Filter object.
*CertMatchingRuleApi* | [**DeleteMsgVpnCertMatchingRuleCondition**](docs/CertMatchingRuleApi.md#deletemsgvpncertmatchingrulecondition) | **DELETE** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions/{source} | Delete a Certificate Matching Rule Condition object.
*CertMatchingRuleApi* | [**GetMsgVpnCertMatchingRule**](docs/CertMatchingRuleApi.md#getmsgvpncertmatchingrule) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Get a Certificate Matching Rule object.
*CertMatchingRuleApi* | [**GetMsgVpnCertMatchingRuleAttributeFilter**](docs/CertMatchingRuleApi.md#getmsgvpncertmatchingruleattributefilter) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Get a Certificate Matching Rule Attribute Filter object.
*CertMatchingRuleApi* | [**GetMsgVpnCertMatchingRuleAttributeFilters**](docs/CertMatchingRuleApi.md#getmsgvpncertmatchingruleattributefilters) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters | Get a list of Certificate Matching Rule Attribute Filter objects.
*CertMatchingRuleApi* | [**GetMsgVpnCertMatchingRuleCondition**](docs/CertMatchingRuleApi.md#getmsgvpncertmatchingrulecondition) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions/{source} | Get a Certificate Matching Rule Condition object.
*CertMatchingRuleApi* | [**GetMsgVpnCertMatchingRuleConditions**](docs/CertMatchingRuleApi.md#getmsgvpncertmatchingruleconditions) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions | Get a list of Certificate Matching Rule Condition objects.
*CertMatchingRuleApi* | [**GetMsgVpnCertMatchingRules**](docs/CertMatchingRuleApi.md#getmsgvpncertmatchingrules) | **GET** /msgVpns/{msgVpnName}/certMatchingRules | Get a list of Certificate Matching Rule objects.
*CertMatchingRuleApi* | [**ReplaceMsgVpnCertMatchingRule**](docs/CertMatchingRuleApi.md#replacemsgvpncertmatchingrule) | **PUT** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Replace a Certificate Matching Rule object.
*CertMatchingRuleApi* | [**ReplaceMsgVpnCertMatchingRuleAttributeFilter**](docs/CertMatchingRuleApi.md#replacemsgvpncertmatchingruleattributefilter) | **PUT** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Replace a Certificate Matching Rule Attribute Filter object.
*CertMatchingRuleApi* | [**UpdateMsgVpnCertMatchingRule**](docs/CertMatchingRuleApi.md#updatemsgvpncertmatchingrule) | **PATCH** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Update a Certificate Matching Rule object.
*CertMatchingRuleApi* | [**UpdateMsgVpnCertMatchingRuleAttributeFilter**](docs/CertMatchingRuleApi.md#updatemsgvpncertmatchingruleattributefilter) | **PATCH** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Update a Certificate Matching Rule Attribute Filter object.
*ClientCertAuthorityApi* | [**CreateClientCertAuthority**](docs/ClientCertAuthorityApi.md#createclientcertauthority) | **POST** /clientCertAuthorities | Create a Client Certificate Authority object.
*ClientCertAuthorityApi* | [**CreateClientCertAuthorityOcspTlsTrustedCommonName**](docs/ClientCertAuthorityApi.md#createclientcertauthorityocsptlstrustedcommonname) | **POST** /clientCertAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames | Create an OCSP Responder Trusted Common Name object.
*ClientCertAuthorityApi* | [**DeleteClientCertAuthority**](docs/ClientCertAuthorityApi.md#deleteclientcertauthority) | **DELETE** /clientCertAuthorities/{certAuthorityName} | Delete a Client Certificate Authority object.
*ClientCertAuthorityApi* | [**DeleteClientCertAuthorityOcspTlsTrustedCommonName**](docs/ClientCertAuthorityApi.md#deleteclientcertauthorityocsptlstrustedcommonname) | **DELETE** /clientCertAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames/{ocspTlsTrustedCommonName} | Delete an OCSP Responder Trusted Common Name object.
*ClientCertAuthorityApi* | [**GetClientCertAuthorities**](docs/ClientCertAuthorityApi.md#getclientcertauthorities) | **GET** /clientCertAuthorities | Get a list of Client Certificate Authority objects.
*ClientCertAuthorityApi* | [**GetClientCertAuthority**](docs/ClientCertAuthorityApi.md#getclientcertauthority) | **GET** /clientCertAuthorities/{certAuthorityName} | Get a Client Certificate Authority object.
*ClientCertAuthorityApi* | [**GetClientCertAuthorityOcspTlsTrustedCommonName**](docs/ClientCertAuthorityApi.md#getclientcertauthorityocsptlstrustedcommonname) | **GET** /clientCertAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames/{ocspTlsTrustedCommonName} | Get an OCSP Responder Trusted Common Name object.
*ClientCertAuthorityApi* | [**GetClientCertAuthorityOcspTlsTrustedCommonNames**](docs/ClientCertAuthorityApi.md#getclientcertauthorityocsptlstrustedcommonnames) | **GET** /clientCertAuthorities/{certAuthorityName}/ocspTlsTrustedCommonNames | Get a list of OCSP Responder Trusted Common Name objects.
*ClientCertAuthorityApi* | [**ReplaceClientCertAuthority**](docs/ClientCertAuthorityApi.md#replaceclientcertauthority) | **PUT** /clientCertAuthorities/{certAuthorityName} | Replace a Client Certificate Authority object.
*ClientCertAuthorityApi* | [**UpdateClientCertAuthority**](docs/ClientCertAuthorityApi.md#updateclientcertauthority) | **PATCH** /clientCertAuthorities/{certAuthorityName} | Update a Client Certificate Authority object.
*ClientProfileApi* | [**CreateMsgVpnClientProfile**](docs/ClientProfileApi.md#createmsgvpnclientprofile) | **POST** /msgVpns/{msgVpnName}/clientProfiles | Create a Client Profile object.
*ClientProfileApi* | [**DeleteMsgVpnClientProfile**](docs/ClientProfileApi.md#deletemsgvpnclientprofile) | **DELETE** /msgVpns/{msgVpnName}/clientProfiles/{clientProfileName} | Delete a Client Profile object.
*ClientProfileApi* | [**GetMsgVpnClientProfile**](docs/ClientProfileApi.md#getmsgvpnclientprofile) | **GET** /msgVpns/{msgVpnName}/clientProfiles/{clientProfileName} | Get a Client Profile object.
*ClientProfileApi* | [**GetMsgVpnClientProfiles**](docs/ClientProfileApi.md#getmsgvpnclientprofiles) | **GET** /msgVpns/{msgVpnName}/clientProfiles | Get a list of Client Profile objects.
*ClientProfileApi* | [**ReplaceMsgVpnClientProfile**](docs/ClientProfileApi.md#replacemsgvpnclientprofile) | **PUT** /msgVpns/{msgVpnName}/clientProfiles/{clientProfileName} | Replace a Client Profile object.
*ClientProfileApi* | [**UpdateMsgVpnClientProfile**](docs/ClientProfileApi.md#updatemsgvpnclientprofile) | **PATCH** /msgVpns/{msgVpnName}/clientProfiles/{clientProfileName} | Update a Client Profile object.
*ClientUsernameApi* | [**CreateMsgVpnClientUsername**](docs/ClientUsernameApi.md#createmsgvpnclientusername) | **POST** /msgVpns/{msgVpnName}/clientUsernames | Create a Client Username object.
*ClientUsernameApi* | [**CreateMsgVpnClientUsernameAttribute**](docs/ClientUsernameApi.md#createmsgvpnclientusernameattribute) | **POST** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername}/attributes | Create a Client Username Attribute object.
*ClientUsernameApi* | [**DeleteMsgVpnClientUsername**](docs/ClientUsernameApi.md#deletemsgvpnclientusername) | **DELETE** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername} | Delete a Client Username object.
*ClientUsernameApi* | [**DeleteMsgVpnClientUsernameAttribute**](docs/ClientUsernameApi.md#deletemsgvpnclientusernameattribute) | **DELETE** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername}/attributes/{attributeName},{attributeValue} | Delete a Client Username Attribute object.
*ClientUsernameApi* | [**GetMsgVpnClientUsername**](docs/ClientUsernameApi.md#getmsgvpnclientusername) | **GET** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername} | Get a Client Username object.
*ClientUsernameApi* | [**GetMsgVpnClientUsernameAttribute**](docs/ClientUsernameApi.md#getmsgvpnclientusernameattribute) | **GET** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername}/attributes/{attributeName},{attributeValue} | Get a Client Username Attribute object.
*ClientUsernameApi* | [**GetMsgVpnClientUsernameAttributes**](docs/ClientUsernameApi.md#getmsgvpnclientusernameattributes) | **GET** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername}/attributes | Get a list of Client Username Attribute objects.
*ClientUsernameApi* | [**GetMsgVpnClientUsernames**](docs/ClientUsernameApi.md#getmsgvpnclientusernames) | **GET** /msgVpns/{msgVpnName}/clientUsernames | Get a list of Client Username objects.
*ClientUsernameApi* | [**ReplaceMsgVpnClientUsername**](docs/ClientUsernameApi.md#replacemsgvpnclientusername) | **PUT** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername} | Replace a Client Username object.
*ClientUsernameApi* | [**UpdateMsgVpnClientUsername**](docs/ClientUsernameApi.md#updatemsgvpnclientusername) | **PATCH** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername} | Update a Client Username object.
*DistributedCacheApi* | [**CreateMsgVpnDistributedCache**](docs/DistributedCacheApi.md#createmsgvpndistributedcache) | **POST** /msgVpns/{msgVpnName}/distributedCaches | Create a Distributed Cache object.
*DistributedCacheApi* | [**CreateMsgVpnDistributedCacheCluster**](docs/DistributedCacheApi.md#createmsgvpndistributedcachecluster) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters | Create a Cache Cluster object.
*DistributedCacheApi* | [**CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**](docs/DistributedCacheApi.md#createmsgvpndistributedcacheclusterglobalcachinghomecluster) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters | Create a Home Cache Cluster object.
*DistributedCacheApi* | [**CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**](docs/DistributedCacheApi.md#createmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes | Create a Topic Prefix object.
*DistributedCacheApi* | [**CreateMsgVpnDistributedCacheClusterInstance**](docs/DistributedCacheApi.md#createmsgvpndistributedcacheclusterinstance) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances | Create a Cache Instance object.
*DistributedCacheApi* | [**CreateMsgVpnDistributedCacheClusterTopic**](docs/DistributedCacheApi.md#createmsgvpndistributedcacheclustertopic) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics | Create a Topic object.
*DistributedCacheApi* | [**DeleteMsgVpnDistributedCache**](docs/DistributedCacheApi.md#deletemsgvpndistributedcache) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Delete a Distributed Cache object.
*DistributedCacheApi* | [**DeleteMsgVpnDistributedCacheCluster**](docs/DistributedCacheApi.md#deletemsgvpndistributedcachecluster) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Delete a Cache Cluster object.
*DistributedCacheApi* | [**DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**](docs/DistributedCacheApi.md#deletemsgvpndistributedcacheclusterglobalcachinghomecluster) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName} | Delete a Home Cache Cluster object.
*DistributedCacheApi* | [**DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**](docs/DistributedCacheApi.md#deletemsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes/{topicPrefix} | Delete a Topic Prefix object.
*DistributedCacheApi* | [**DeleteMsgVpnDistributedCacheClusterInstance**](docs/DistributedCacheApi.md#deletemsgvpndistributedcacheclusterinstance) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Delete a Cache Instance object.
*DistributedCacheApi* | [**DeleteMsgVpnDistributedCacheClusterTopic**](docs/DistributedCacheApi.md#deletemsgvpndistributedcacheclustertopic) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics/{topic} | Delete a Topic object.
*DistributedCacheApi* | [**GetMsgVpnDistributedCache**](docs/DistributedCacheApi.md#getmsgvpndistributedcache) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Get a Distributed Cache object.
*DistributedCacheApi* | [**GetMsgVpnDistributedCacheCluster**](docs/DistributedCacheApi.md#getmsgvpndistributedcachecluster) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Get a Cache Cluster object.
*DistributedCacheApi* | [**GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**](docs/DistributedCacheApi.md#getmsgvpndistributedcacheclusterglobalcachinghomecluster) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName} | Get a Home Cache Cluster object.
*DistributedCacheApi* | [**GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**](docs/DistributedCacheApi.md#getmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes/{topicPrefix} | Get a Topic Prefix object.
*DistributedCacheApi* | [**GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes**](docs/DistributedCacheApi.md#getmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefixes) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes | Get a list of Topic Prefix objects.
*DistributedCacheApi* | [**GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters**](docs/DistributedCacheApi.md#getmsgvpndistributedcacheclusterglobalcachinghomeclusters) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters | Get a list of Home Cache Cluster objects.
*DistributedCacheApi* | [**GetMsgVpnDistributedCacheClusterInstance**](docs/DistributedCacheApi.md#getmsgvpndistributedcacheclusterinstance) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Get a Cache Instance object.
*DistributedCacheApi* | [**GetMsgVpnDistributedCacheClusterInstances**](docs/DistributedCacheApi.md#getmsgvpndistributedcacheclusterinstances) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances | Get a list of Cache Instance objects.
*DistributedCacheApi* | [**GetMsgVpnDistributedCacheClusterTopic**](docs/DistributedCacheApi.md#getmsgvpndistributedcacheclustertopic) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics/{topic} | Get a Topic object.
*DistributedCacheApi* | [**GetMsgVpnDistributedCacheClusterTopics**](docs/DistributedCacheApi.md#getmsgvpndistributedcacheclustertopics) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics | Get a list of Topic objects.
*DistributedCacheApi* | [**GetMsgVpnDistributedCacheClusters**](docs/DistributedCacheApi.md#getmsgvpndistributedcacheclusters) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters | Get a list of Cache Cluster objects.
*DistributedCacheApi* | [**GetMsgVpnDistributedCaches**](docs/DistributedCacheApi.md#getmsgvpndistributedcaches) | **GET** /msgVpns/{msgVpnName}/distributedCaches | Get a list of Distributed Cache objects.
*DistributedCacheApi* | [**ReplaceMsgVpnDistributedCache**](docs/DistributedCacheApi.md#replacemsgvpndistributedcache) | **PUT** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Replace a Distributed Cache object.
*DistributedCacheApi* | [**ReplaceMsgVpnDistributedCacheCluster**](docs/DistributedCacheApi.md#replacemsgvpndistributedcachecluster) | **PUT** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Replace a Cache Cluster object.
*DistributedCacheApi* | [**ReplaceMsgVpnDistributedCacheClusterInstance**](docs/DistributedCacheApi.md#replacemsgvpndistributedcacheclusterinstance) | **PUT** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Replace a Cache Instance object.
*DistributedCacheApi* | [**UpdateMsgVpnDistributedCache**](docs/DistributedCacheApi.md#updatemsgvpndistributedcache) | **PATCH** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Update a Distributed Cache object.
*DistributedCacheApi* | [**UpdateMsgVpnDistributedCacheCluster**](docs/DistributedCacheApi.md#updatemsgvpndistributedcachecluster) | **PATCH** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Update a Cache Cluster object.
*DistributedCacheApi* | [**UpdateMsgVpnDistributedCacheClusterInstance**](docs/DistributedCacheApi.md#updatemsgvpndistributedcacheclusterinstance) | **PATCH** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Update a Cache Instance object.
*DmrBridgeApi* | [**CreateMsgVpnDmrBridge**](docs/DmrBridgeApi.md#createmsgvpndmrbridge) | **POST** /msgVpns/{msgVpnName}/dmrBridges | Create a DMR Bridge object.
*DmrBridgeApi* | [**DeleteMsgVpnDmrBridge**](docs/DmrBridgeApi.md#deletemsgvpndmrbridge) | **DELETE** /msgVpns/{msgVpnName}/dmrBridges/{remoteNodeName} | Delete a DMR Bridge object.
*DmrBridgeApi* | [**GetMsgVpnDmrBridge**](docs/DmrBridgeApi.md#getmsgvpndmrbridge) | **GET** /msgVpns/{msgVpnName}/dmrBridges/{remoteNodeName} | Get a DMR Bridge object.
*DmrBridgeApi* | [**GetMsgVpnDmrBridges**](docs/DmrBridgeApi.md#getmsgvpndmrbridges) | **GET** /msgVpns/{msgVpnName}/dmrBridges | Get a list of DMR Bridge objects.
*DmrBridgeApi* | [**ReplaceMsgVpnDmrBridge**](docs/DmrBridgeApi.md#replacemsgvpndmrbridge) | **PUT** /msgVpns/{msgVpnName}/dmrBridges/{remoteNodeName} | Replace a DMR Bridge object.
*DmrBridgeApi* | [**UpdateMsgVpnDmrBridge**](docs/DmrBridgeApi.md#updatemsgvpndmrbridge) | **PATCH** /msgVpns/{msgVpnName}/dmrBridges/{remoteNodeName} | Update a DMR Bridge object.
*DmrClusterApi* | [**CreateDmrCluster**](docs/DmrClusterApi.md#createdmrcluster) | **POST** /dmrClusters | Create a Cluster object.
*DmrClusterApi* | [**CreateDmrClusterCertMatchingRule**](docs/DmrClusterApi.md#createdmrclustercertmatchingrule) | **POST** /dmrClusters/{dmrClusterName}/certMatchingRules | Create a Certificate Matching Rule object.
*DmrClusterApi* | [**CreateDmrClusterCertMatchingRuleAttributeFilter**](docs/DmrClusterApi.md#createdmrclustercertmatchingruleattributefilter) | **POST** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters | Create a Certificate Matching Rule Attribute Filter object.
*DmrClusterApi* | [**CreateDmrClusterCertMatchingRuleCondition**](docs/DmrClusterApi.md#createdmrclustercertmatchingrulecondition) | **POST** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions | Create a Certificate Matching Rule Condition object.
*DmrClusterApi* | [**CreateDmrClusterLink**](docs/DmrClusterApi.md#createdmrclusterlink) | **POST** /dmrClusters/{dmrClusterName}/links | Create a Link object.
*DmrClusterApi* | [**CreateDmrClusterLinkAttribute**](docs/DmrClusterApi.md#createdmrclusterlinkattribute) | **POST** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes | Create a Link Attribute object.
*DmrClusterApi* | [**CreateDmrClusterLinkRemoteAddress**](docs/DmrClusterApi.md#createdmrclusterlinkremoteaddress) | **POST** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses | Create a Remote Address object.
*DmrClusterApi* | [**CreateDmrClusterLinkTlsTrustedCommonName**](docs/DmrClusterApi.md#createdmrclusterlinktlstrustedcommonname) | **POST** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames | Create a Trusted Common Name object.
*DmrClusterApi* | [**DeleteDmrCluster**](docs/DmrClusterApi.md#deletedmrcluster) | **DELETE** /dmrClusters/{dmrClusterName} | Delete a Cluster object.
*DmrClusterApi* | [**DeleteDmrClusterCertMatchingRule**](docs/DmrClusterApi.md#deletedmrclustercertmatchingrule) | **DELETE** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName} | Delete a Certificate Matching Rule object.
*DmrClusterApi* | [**DeleteDmrClusterCertMatchingRuleAttributeFilter**](docs/DmrClusterApi.md#deletedmrclustercertmatchingruleattributefilter) | **DELETE** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Delete a Certificate Matching Rule Attribute Filter object.
*DmrClusterApi* | [**DeleteDmrClusterCertMatchingRuleCondition**](docs/DmrClusterApi.md#deletedmrclustercertmatchingrulecondition) | **DELETE** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions/{source} | Delete a Certificate Matching Rule Condition object.
*DmrClusterApi* | [**DeleteDmrClusterLink**](docs/DmrClusterApi.md#deletedmrclusterlink) | **DELETE** /dmrClusters/{dmrClusterName}/links/{remoteNodeName} | Delete a Link object.
*DmrClusterApi* | [**DeleteDmrClusterLinkAttribute**](docs/DmrClusterApi.md#deletedmrclusterlinkattribute) | **DELETE** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes/{attributeName},{attributeValue} | Delete a Link Attribute object.
*DmrClusterApi* | [**DeleteDmrClusterLinkRemoteAddress**](docs/DmrClusterApi.md#deletedmrclusterlinkremoteaddress) | **DELETE** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses/{remoteAddress} | Delete a Remote Address object.
*DmrClusterApi* | [**DeleteDmrClusterLinkTlsTrustedCommonName**](docs/DmrClusterApi.md#deletedmrclusterlinktlstrustedcommonname) | **DELETE** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Delete a Trusted Common Name object.
*DmrClusterApi* | [**GetDmrCluster**](docs/DmrClusterApi.md#getdmrcluster) | **GET** /dmrClusters/{dmrClusterName} | Get a Cluster object.
*DmrClusterApi* | [**GetDmrClusterCertMatchingRule**](docs/DmrClusterApi.md#getdmrclustercertmatchingrule) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName} | Get a Certificate Matching Rule object.
*DmrClusterApi* | [**GetDmrClusterCertMatchingRuleAttributeFilter**](docs/DmrClusterApi.md#getdmrclustercertmatchingruleattributefilter) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Get a Certificate Matching Rule Attribute Filter object.
*DmrClusterApi* | [**GetDmrClusterCertMatchingRuleAttributeFilters**](docs/DmrClusterApi.md#getdmrclustercertmatchingruleattributefilters) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters | Get a list of Certificate Matching Rule Attribute Filter objects.
*DmrClusterApi* | [**GetDmrClusterCertMatchingRuleCondition**](docs/DmrClusterApi.md#getdmrclustercertmatchingrulecondition) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions/{source} | Get a Certificate Matching Rule Condition object.
*DmrClusterApi* | [**GetDmrClusterCertMatchingRuleConditions**](docs/DmrClusterApi.md#getdmrclustercertmatchingruleconditions) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/conditions | Get a list of Certificate Matching Rule Condition objects.
*DmrClusterApi* | [**GetDmrClusterCertMatchingRules**](docs/DmrClusterApi.md#getdmrclustercertmatchingrules) | **GET** /dmrClusters/{dmrClusterName}/certMatchingRules | Get a list of Certificate Matching Rule objects.
*DmrClusterApi* | [**GetDmrClusterLink**](docs/DmrClusterApi.md#getdmrclusterlink) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName} | Get a Link object.
*DmrClusterApi* | [**GetDmrClusterLinkAttribute**](docs/DmrClusterApi.md#getdmrclusterlinkattribute) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes/{attributeName},{attributeValue} | Get a Link Attribute object.
*DmrClusterApi* | [**GetDmrClusterLinkAttributes**](docs/DmrClusterApi.md#getdmrclusterlinkattributes) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/attributes | Get a list of Link Attribute objects.
*DmrClusterApi* | [**GetDmrClusterLinkRemoteAddress**](docs/DmrClusterApi.md#getdmrclusterlinkremoteaddress) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses/{remoteAddress} | Get a Remote Address object.
*DmrClusterApi* | [**GetDmrClusterLinkRemoteAddresses**](docs/DmrClusterApi.md#getdmrclusterlinkremoteaddresses) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/remoteAddresses | Get a list of Remote Address objects.
*DmrClusterApi* | [**GetDmrClusterLinkTlsTrustedCommonName**](docs/DmrClusterApi.md#getdmrclusterlinktlstrustedcommonname) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Get a Trusted Common Name object.
*DmrClusterApi* | [**GetDmrClusterLinkTlsTrustedCommonNames**](docs/DmrClusterApi.md#getdmrclusterlinktlstrustedcommonnames) | **GET** /dmrClusters/{dmrClusterName}/links/{remoteNodeName}/tlsTrustedCommonNames | Get a list of Trusted Common Name objects.
*DmrClusterApi* | [**GetDmrClusterLinks**](docs/DmrClusterApi.md#getdmrclusterlinks) | **GET** /dmrClusters/{dmrClusterName}/links | Get a list of Link objects.
*DmrClusterApi* | [**GetDmrClusters**](docs/DmrClusterApi.md#getdmrclusters) | **GET** /dmrClusters | Get a list of Cluster objects.
*DmrClusterApi* | [**ReplaceDmrCluster**](docs/DmrClusterApi.md#replacedmrcluster) | **PUT** /dmrClusters/{dmrClusterName} | Replace a Cluster object.
*DmrClusterApi* | [**ReplaceDmrClusterCertMatchingRule**](docs/DmrClusterApi.md#replacedmrclustercertmatchingrule) | **PUT** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName} | Replace a Certificate Matching Rule object.
*DmrClusterApi* | [**ReplaceDmrClusterCertMatchingRuleAttributeFilter**](docs/DmrClusterApi.md#replacedmrclustercertmatchingruleattributefilter) | **PUT** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Replace a Certificate Matching Rule Attribute Filter object.
*DmrClusterApi* | [**ReplaceDmrClusterLink**](docs/DmrClusterApi.md#replacedmrclusterlink) | **PUT** /dmrClusters/{dmrClusterName}/links/{remoteNodeName} | Replace a Link object.
*DmrClusterApi* | [**UpdateDmrCluster**](docs/DmrClusterApi.md#updatedmrcluster) | **PATCH** /dmrClusters/{dmrClusterName} | Update a Cluster object.
*DmrClusterApi* | [**UpdateDmrClusterCertMatchingRule**](docs/DmrClusterApi.md#updatedmrclustercertmatchingrule) | **PATCH** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName} | Update a Certificate Matching Rule object.
*DmrClusterApi* | [**UpdateDmrClusterCertMatchingRuleAttributeFilter**](docs/DmrClusterApi.md#updatedmrclustercertmatchingruleattributefilter) | **PATCH** /dmrClusters/{dmrClusterName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Update a Certificate Matching Rule Attribute Filter object.
*DmrClusterApi* | [**UpdateDmrClusterLink**](docs/DmrClusterApi.md#updatedmrclusterlink) | **PATCH** /dmrClusters/{dmrClusterName}/links/{remoteNodeName} | Update a Link object.
*DomainCertAuthorityApi* | [**CreateDomainCertAuthority**](docs/DomainCertAuthorityApi.md#createdomaincertauthority) | **POST** /domainCertAuthorities | Create a Domain Certificate Authority object.
*DomainCertAuthorityApi* | [**DeleteDomainCertAuthority**](docs/DomainCertAuthorityApi.md#deletedomaincertauthority) | **DELETE** /domainCertAuthorities/{certAuthorityName} | Delete a Domain Certificate Authority object.
*DomainCertAuthorityApi* | [**GetDomainCertAuthorities**](docs/DomainCertAuthorityApi.md#getdomaincertauthorities) | **GET** /domainCertAuthorities | Get a list of Domain Certificate Authority objects.
*DomainCertAuthorityApi* | [**GetDomainCertAuthority**](docs/DomainCertAuthorityApi.md#getdomaincertauthority) | **GET** /domainCertAuthorities/{certAuthorityName} | Get a Domain Certificate Authority object.
*DomainCertAuthorityApi* | [**ReplaceDomainCertAuthority**](docs/DomainCertAuthorityApi.md#replacedomaincertauthority) | **PUT** /domainCertAuthorities/{certAuthorityName} | Replace a Domain Certificate Authority object.
*DomainCertAuthorityApi* | [**UpdateDomainCertAuthority**](docs/DomainCertAuthorityApi.md#updatedomaincertauthority) | **PATCH** /domainCertAuthorities/{certAuthorityName} | Update a Domain Certificate Authority object.
*JndiApi* | [**CreateMsgVpnJndiConnectionFactory**](docs/JndiApi.md#createmsgvpnjndiconnectionfactory) | **POST** /msgVpns/{msgVpnName}/jndiConnectionFactories | Create a JNDI Connection Factory object.
*JndiApi* | [**CreateMsgVpnJndiQueue**](docs/JndiApi.md#createmsgvpnjndiqueue) | **POST** /msgVpns/{msgVpnName}/jndiQueues | Create a JNDI Queue object.
*JndiApi* | [**CreateMsgVpnJndiTopic**](docs/JndiApi.md#createmsgvpnjnditopic) | **POST** /msgVpns/{msgVpnName}/jndiTopics | Create a JNDI Topic object.
*JndiApi* | [**DeleteMsgVpnJndiConnectionFactory**](docs/JndiApi.md#deletemsgvpnjndiconnectionfactory) | **DELETE** /msgVpns/{msgVpnName}/jndiConnectionFactories/{connectionFactoryName} | Delete a JNDI Connection Factory object.
*JndiApi* | [**DeleteMsgVpnJndiQueue**](docs/JndiApi.md#deletemsgvpnjndiqueue) | **DELETE** /msgVpns/{msgVpnName}/jndiQueues/{queueName} | Delete a JNDI Queue object.
*JndiApi* | [**DeleteMsgVpnJndiTopic**](docs/JndiApi.md#deletemsgvpnjnditopic) | **DELETE** /msgVpns/{msgVpnName}/jndiTopics/{topicName} | Delete a JNDI Topic object.
*JndiApi* | [**GetMsgVpnJndiConnectionFactories**](docs/JndiApi.md#getmsgvpnjndiconnectionfactories) | **GET** /msgVpns/{msgVpnName}/jndiConnectionFactories | Get a list of JNDI Connection Factory objects.
*JndiApi* | [**GetMsgVpnJndiConnectionFactory**](docs/JndiApi.md#getmsgvpnjndiconnectionfactory) | **GET** /msgVpns/{msgVpnName}/jndiConnectionFactories/{connectionFactoryName} | Get a JNDI Connection Factory object.
*JndiApi* | [**GetMsgVpnJndiQueue**](docs/JndiApi.md#getmsgvpnjndiqueue) | **GET** /msgVpns/{msgVpnName}/jndiQueues/{queueName} | Get a JNDI Queue object.
*JndiApi* | [**GetMsgVpnJndiQueues**](docs/JndiApi.md#getmsgvpnjndiqueues) | **GET** /msgVpns/{msgVpnName}/jndiQueues | Get a list of JNDI Queue objects.
*JndiApi* | [**GetMsgVpnJndiTopic**](docs/JndiApi.md#getmsgvpnjnditopic) | **GET** /msgVpns/{msgVpnName}/jndiTopics/{topicName} | Get a JNDI Topic object.
*JndiApi* | [**GetMsgVpnJndiTopics**](docs/JndiApi.md#getmsgvpnjnditopics) | **GET** /msgVpns/{msgVpnName}/jndiTopics | Get a list of JNDI Topic objects.
*JndiApi* | [**ReplaceMsgVpnJndiConnectionFactory**](docs/JndiApi.md#replacemsgvpnjndiconnectionfactory) | **PUT** /msgVpns/{msgVpnName}/jndiConnectionFactories/{connectionFactoryName} | Replace a JNDI Connection Factory object.
*JndiApi* | [**ReplaceMsgVpnJndiQueue**](docs/JndiApi.md#replacemsgvpnjndiqueue) | **PUT** /msgVpns/{msgVpnName}/jndiQueues/{queueName} | Replace a JNDI Queue object.
*JndiApi* | [**ReplaceMsgVpnJndiTopic**](docs/JndiApi.md#replacemsgvpnjnditopic) | **PUT** /msgVpns/{msgVpnName}/jndiTopics/{topicName} | Replace a JNDI Topic object.
*JndiApi* | [**UpdateMsgVpnJndiConnectionFactory**](docs/JndiApi.md#updatemsgvpnjndiconnectionfactory) | **PATCH** /msgVpns/{msgVpnName}/jndiConnectionFactories/{connectionFactoryName} | Update a JNDI Connection Factory object.
*JndiApi* | [**UpdateMsgVpnJndiQueue**](docs/JndiApi.md#updatemsgvpnjndiqueue) | **PATCH** /msgVpns/{msgVpnName}/jndiQueues/{queueName} | Update a JNDI Queue object.
*JndiApi* | [**UpdateMsgVpnJndiTopic**](docs/JndiApi.md#updatemsgvpnjnditopic) | **PATCH** /msgVpns/{msgVpnName}/jndiTopics/{topicName} | Update a JNDI Topic object.
*MqttRetainCacheApi* | [**CreateMsgVpnMqttRetainCache**](docs/MqttRetainCacheApi.md#createmsgvpnmqttretaincache) | **POST** /msgVpns/{msgVpnName}/mqttRetainCaches | Create an MQTT Retain Cache object.
*MqttRetainCacheApi* | [**DeleteMsgVpnMqttRetainCache**](docs/MqttRetainCacheApi.md#deletemsgvpnmqttretaincache) | **DELETE** /msgVpns/{msgVpnName}/mqttRetainCaches/{cacheName} | Delete an MQTT Retain Cache object.
*MqttRetainCacheApi* | [**GetMsgVpnMqttRetainCache**](docs/MqttRetainCacheApi.md#getmsgvpnmqttretaincache) | **GET** /msgVpns/{msgVpnName}/mqttRetainCaches/{cacheName} | Get an MQTT Retain Cache object.
*MqttRetainCacheApi* | [**GetMsgVpnMqttRetainCaches**](docs/MqttRetainCacheApi.md#getmsgvpnmqttretaincaches) | **GET** /msgVpns/{msgVpnName}/mqttRetainCaches | Get a list of MQTT Retain Cache objects.
*MqttRetainCacheApi* | [**ReplaceMsgVpnMqttRetainCache**](docs/MqttRetainCacheApi.md#replacemsgvpnmqttretaincache) | **PUT** /msgVpns/{msgVpnName}/mqttRetainCaches/{cacheName} | Replace an MQTT Retain Cache object.
*MqttRetainCacheApi* | [**UpdateMsgVpnMqttRetainCache**](docs/MqttRetainCacheApi.md#updatemsgvpnmqttretaincache) | **PATCH** /msgVpns/{msgVpnName}/mqttRetainCaches/{cacheName} | Update an MQTT Retain Cache object.
*MqttSessionApi* | [**CreateMsgVpnMqttSession**](docs/MqttSessionApi.md#createmsgvpnmqttsession) | **POST** /msgVpns/{msgVpnName}/mqttSessions | Create an MQTT Session object.
*MqttSessionApi* | [**CreateMsgVpnMqttSessionSubscription**](docs/MqttSessionApi.md#createmsgvpnmqttsessionsubscription) | **POST** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions | Create a Subscription object.
*MqttSessionApi* | [**DeleteMsgVpnMqttSession**](docs/MqttSessionApi.md#deletemsgvpnmqttsession) | **DELETE** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter} | Delete an MQTT Session object.
*MqttSessionApi* | [**DeleteMsgVpnMqttSessionSubscription**](docs/MqttSessionApi.md#deletemsgvpnmqttsessionsubscription) | **DELETE** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions/{subscriptionTopic} | Delete a Subscription object.
*MqttSessionApi* | [**GetMsgVpnMqttSession**](docs/MqttSessionApi.md#getmsgvpnmqttsession) | **GET** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter} | Get an MQTT Session object.
*MqttSessionApi* | [**GetMsgVpnMqttSessionSubscription**](docs/MqttSessionApi.md#getmsgvpnmqttsessionsubscription) | **GET** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions/{subscriptionTopic} | Get a Subscription object.
*MqttSessionApi* | [**GetMsgVpnMqttSessionSubscriptions**](docs/MqttSessionApi.md#getmsgvpnmqttsessionsubscriptions) | **GET** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions | Get a list of Subscription objects.
*MqttSessionApi* | [**GetMsgVpnMqttSessions**](docs/MqttSessionApi.md#getmsgvpnmqttsessions) | **GET** /msgVpns/{msgVpnName}/mqttSessions | Get a list of MQTT Session objects.
*MqttSessionApi* | [**ReplaceMsgVpnMqttSession**](docs/MqttSessionApi.md#replacemsgvpnmqttsession) | **PUT** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter} | Replace an MQTT Session object.
*MqttSessionApi* | [**ReplaceMsgVpnMqttSessionSubscription**](docs/MqttSessionApi.md#replacemsgvpnmqttsessionsubscription) | **PUT** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions/{subscriptionTopic} | Replace a Subscription object.
*MqttSessionApi* | [**UpdateMsgVpnMqttSession**](docs/MqttSessionApi.md#updatemsgvpnmqttsession) | **PATCH** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter} | Update an MQTT Session object.
*MqttSessionApi* | [**UpdateMsgVpnMqttSessionSubscription**](docs/MqttSessionApi.md#updatemsgvpnmqttsessionsubscription) | **PATCH** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions/{subscriptionTopic} | Update a Subscription object.
*MsgVpnApi* | [**CreateMsgVpn**](docs/MsgVpnApi.md#createmsgvpn) | **POST** /msgVpns | Create a Message VPN object.
*MsgVpnApi* | [**CreateMsgVpnAclProfile**](docs/MsgVpnApi.md#createmsgvpnaclprofile) | **POST** /msgVpns/{msgVpnName}/aclProfiles | Create an ACL Profile object.
*MsgVpnApi* | [**CreateMsgVpnAclProfileClientConnectException**](docs/MsgVpnApi.md#createmsgvpnaclprofileclientconnectexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions | Create a Client Connect Exception object.
*MsgVpnApi* | [**CreateMsgVpnAclProfilePublishException**](docs/MsgVpnApi.md#createmsgvpnaclprofilepublishexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions | Create a Publish Topic Exception object.
*MsgVpnApi* | [**CreateMsgVpnAclProfilePublishTopicException**](docs/MsgVpnApi.md#createmsgvpnaclprofilepublishtopicexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions | Create a Publish Topic Exception object.
*MsgVpnApi* | [**CreateMsgVpnAclProfileSubscribeException**](docs/MsgVpnApi.md#createmsgvpnaclprofilesubscribeexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions | Create a Subscribe Topic Exception object.
*MsgVpnApi* | [**CreateMsgVpnAclProfileSubscribeShareNameException**](docs/MsgVpnApi.md#createmsgvpnaclprofilesubscribesharenameexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions | Create a Subscribe Share Name Exception object.
*MsgVpnApi* | [**CreateMsgVpnAclProfileSubscribeTopicException**](docs/MsgVpnApi.md#createmsgvpnaclprofilesubscribetopicexception) | **POST** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions | Create a Subscribe Topic Exception object.
*MsgVpnApi* | [**CreateMsgVpnAuthenticationOauthProfile**](docs/MsgVpnApi.md#createmsgvpnauthenticationoauthprofile) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProfiles | Create an OAuth Profile object.
*MsgVpnApi* | [**CreateMsgVpnAuthenticationOauthProfileClientRequiredClaim**](docs/MsgVpnApi.md#createmsgvpnauthenticationoauthprofileclientrequiredclaim) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims | Create a Required Claim object.
*MsgVpnApi* | [**CreateMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**](docs/MsgVpnApi.md#createmsgvpnauthenticationoauthprofileresourceserverrequiredclaim) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Create a Required Claim object.
*MsgVpnApi* | [**CreateMsgVpnAuthenticationOauthProvider**](docs/MsgVpnApi.md#createmsgvpnauthenticationoauthprovider) | **POST** /msgVpns/{msgVpnName}/authenticationOauthProviders | Create an OAuth Provider object.
*MsgVpnApi* | [**CreateMsgVpnAuthorizationGroup**](docs/MsgVpnApi.md#createmsgvpnauthorizationgroup) | **POST** /msgVpns/{msgVpnName}/authorizationGroups | Create an Authorization Group object.
*MsgVpnApi* | [**CreateMsgVpnBridge**](docs/MsgVpnApi.md#createmsgvpnbridge) | **POST** /msgVpns/{msgVpnName}/bridges | Create a Bridge object.
*MsgVpnApi* | [**CreateMsgVpnBridgeRemoteMsgVpn**](docs/MsgVpnApi.md#createmsgvpnbridgeremotemsgvpn) | **POST** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns | Create a Remote Message VPN object.
*MsgVpnApi* | [**CreateMsgVpnBridgeRemoteSubscription**](docs/MsgVpnApi.md#createmsgvpnbridgeremotesubscription) | **POST** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions | Create a Remote Subscription object.
*MsgVpnApi* | [**CreateMsgVpnBridgeTlsTrustedCommonName**](docs/MsgVpnApi.md#createmsgvpnbridgetlstrustedcommonname) | **POST** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames | Create a Trusted Common Name object.
*MsgVpnApi* | [**CreateMsgVpnCertMatchingRule**](docs/MsgVpnApi.md#createmsgvpncertmatchingrule) | **POST** /msgVpns/{msgVpnName}/certMatchingRules | Create a Certificate Matching Rule object.
*MsgVpnApi* | [**CreateMsgVpnCertMatchingRuleAttributeFilter**](docs/MsgVpnApi.md#createmsgvpncertmatchingruleattributefilter) | **POST** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters | Create a Certificate Matching Rule Attribute Filter object.
*MsgVpnApi* | [**CreateMsgVpnCertMatchingRuleCondition**](docs/MsgVpnApi.md#createmsgvpncertmatchingrulecondition) | **POST** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions | Create a Certificate Matching Rule Condition object.
*MsgVpnApi* | [**CreateMsgVpnClientProfile**](docs/MsgVpnApi.md#createmsgvpnclientprofile) | **POST** /msgVpns/{msgVpnName}/clientProfiles | Create a Client Profile object.
*MsgVpnApi* | [**CreateMsgVpnClientUsername**](docs/MsgVpnApi.md#createmsgvpnclientusername) | **POST** /msgVpns/{msgVpnName}/clientUsernames | Create a Client Username object.
*MsgVpnApi* | [**CreateMsgVpnClientUsernameAttribute**](docs/MsgVpnApi.md#createmsgvpnclientusernameattribute) | **POST** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername}/attributes | Create a Client Username Attribute object.
*MsgVpnApi* | [**CreateMsgVpnDistributedCache**](docs/MsgVpnApi.md#createmsgvpndistributedcache) | **POST** /msgVpns/{msgVpnName}/distributedCaches | Create a Distributed Cache object.
*MsgVpnApi* | [**CreateMsgVpnDistributedCacheCluster**](docs/MsgVpnApi.md#createmsgvpndistributedcachecluster) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters | Create a Cache Cluster object.
*MsgVpnApi* | [**CreateMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**](docs/MsgVpnApi.md#createmsgvpndistributedcacheclusterglobalcachinghomecluster) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters | Create a Home Cache Cluster object.
*MsgVpnApi* | [**CreateMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**](docs/MsgVpnApi.md#createmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes | Create a Topic Prefix object.
*MsgVpnApi* | [**CreateMsgVpnDistributedCacheClusterInstance**](docs/MsgVpnApi.md#createmsgvpndistributedcacheclusterinstance) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances | Create a Cache Instance object.
*MsgVpnApi* | [**CreateMsgVpnDistributedCacheClusterTopic**](docs/MsgVpnApi.md#createmsgvpndistributedcacheclustertopic) | **POST** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics | Create a Topic object.
*MsgVpnApi* | [**CreateMsgVpnDmrBridge**](docs/MsgVpnApi.md#createmsgvpndmrbridge) | **POST** /msgVpns/{msgVpnName}/dmrBridges | Create a DMR Bridge object.
*MsgVpnApi* | [**CreateMsgVpnJndiConnectionFactory**](docs/MsgVpnApi.md#createmsgvpnjndiconnectionfactory) | **POST** /msgVpns/{msgVpnName}/jndiConnectionFactories | Create a JNDI Connection Factory object.
*MsgVpnApi* | [**CreateMsgVpnJndiQueue**](docs/MsgVpnApi.md#createmsgvpnjndiqueue) | **POST** /msgVpns/{msgVpnName}/jndiQueues | Create a JNDI Queue object.
*MsgVpnApi* | [**CreateMsgVpnJndiTopic**](docs/MsgVpnApi.md#createmsgvpnjnditopic) | **POST** /msgVpns/{msgVpnName}/jndiTopics | Create a JNDI Topic object.
*MsgVpnApi* | [**CreateMsgVpnMqttRetainCache**](docs/MsgVpnApi.md#createmsgvpnmqttretaincache) | **POST** /msgVpns/{msgVpnName}/mqttRetainCaches | Create an MQTT Retain Cache object.
*MsgVpnApi* | [**CreateMsgVpnMqttSession**](docs/MsgVpnApi.md#createmsgvpnmqttsession) | **POST** /msgVpns/{msgVpnName}/mqttSessions | Create an MQTT Session object.
*MsgVpnApi* | [**CreateMsgVpnMqttSessionSubscription**](docs/MsgVpnApi.md#createmsgvpnmqttsessionsubscription) | **POST** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions | Create a Subscription object.
*MsgVpnApi* | [**CreateMsgVpnQueue**](docs/MsgVpnApi.md#createmsgvpnqueue) | **POST** /msgVpns/{msgVpnName}/queues | Create a Queue object.
*MsgVpnApi* | [**CreateMsgVpnQueueSubscription**](docs/MsgVpnApi.md#createmsgvpnqueuesubscription) | **POST** /msgVpns/{msgVpnName}/queues/{queueName}/subscriptions | Create a Queue Subscription object.
*MsgVpnApi* | [**CreateMsgVpnQueueTemplate**](docs/MsgVpnApi.md#createmsgvpnqueuetemplate) | **POST** /msgVpns/{msgVpnName}/queueTemplates | Create a Queue Template object.
*MsgVpnApi* | [**CreateMsgVpnReplayLog**](docs/MsgVpnApi.md#createmsgvpnreplaylog) | **POST** /msgVpns/{msgVpnName}/replayLogs | Create a Replay Log object.
*MsgVpnApi* | [**CreateMsgVpnReplayLogTopicFilterSubscription**](docs/MsgVpnApi.md#createmsgvpnreplaylogtopicfiltersubscription) | **POST** /msgVpns/{msgVpnName}/replayLogs/{replayLogName}/topicFilterSubscriptions | Create a Topic Filter Subscription object.
*MsgVpnApi* | [**CreateMsgVpnReplicatedTopic**](docs/MsgVpnApi.md#createmsgvpnreplicatedtopic) | **POST** /msgVpns/{msgVpnName}/replicatedTopics | Create a Replicated Topic object.
*MsgVpnApi* | [**CreateMsgVpnRestDeliveryPoint**](docs/MsgVpnApi.md#createmsgvpnrestdeliverypoint) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints | Create a REST Delivery Point object.
*MsgVpnApi* | [**CreateMsgVpnRestDeliveryPointQueueBinding**](docs/MsgVpnApi.md#createmsgvpnrestdeliverypointqueuebinding) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings | Create a Queue Binding object.
*MsgVpnApi* | [**CreateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/MsgVpnApi.md#createmsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders | Create a Protected Request Header object.
*MsgVpnApi* | [**CreateMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/MsgVpnApi.md#createmsgvpnrestdeliverypointqueuebindingrequestheader) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders | Create a Request Header object.
*MsgVpnApi* | [**CreateMsgVpnRestDeliveryPointRestConsumer**](docs/MsgVpnApi.md#createmsgvpnrestdeliverypointrestconsumer) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers | Create a REST Consumer object.
*MsgVpnApi* | [**CreateMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**](docs/MsgVpnApi.md#createmsgvpnrestdeliverypointrestconsumeroauthjwtclaim) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims | Create a Claim object.
*MsgVpnApi* | [**CreateMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**](docs/MsgVpnApi.md#createmsgvpnrestdeliverypointrestconsumertlstrustedcommonname) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames | Create a Trusted Common Name object.
*MsgVpnApi* | [**CreateMsgVpnSequencedTopic**](docs/MsgVpnApi.md#createmsgvpnsequencedtopic) | **POST** /msgVpns/{msgVpnName}/sequencedTopics | Create a Sequenced Topic object.
*MsgVpnApi* | [**CreateMsgVpnTelemetryProfile**](docs/MsgVpnApi.md#createmsgvpntelemetryprofile) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles | Create a Telemetry Profile object.
*MsgVpnApi* | [**CreateMsgVpnTelemetryProfileReceiverAclConnectException**](docs/MsgVpnApi.md#createmsgvpntelemetryprofilereceiveraclconnectexception) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions | Create a Receiver ACL Connect Exception object.
*MsgVpnApi* | [**CreateMsgVpnTelemetryProfileTraceFilter**](docs/MsgVpnApi.md#createmsgvpntelemetryprofiletracefilter) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters | Create a Trace Filter object.
*MsgVpnApi* | [**CreateMsgVpnTelemetryProfileTraceFilterSubscription**](docs/MsgVpnApi.md#createmsgvpntelemetryprofiletracefiltersubscription) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions | Create a Telemetry Trace Filter Subscription object.
*MsgVpnApi* | [**CreateMsgVpnTopicEndpoint**](docs/MsgVpnApi.md#createmsgvpntopicendpoint) | **POST** /msgVpns/{msgVpnName}/topicEndpoints | Create a Topic Endpoint object.
*MsgVpnApi* | [**CreateMsgVpnTopicEndpointTemplate**](docs/MsgVpnApi.md#createmsgvpntopicendpointtemplate) | **POST** /msgVpns/{msgVpnName}/topicEndpointTemplates | Create a Topic Endpoint Template object.
*MsgVpnApi* | [**DeleteMsgVpn**](docs/MsgVpnApi.md#deletemsgvpn) | **DELETE** /msgVpns/{msgVpnName} | Delete a Message VPN object.
*MsgVpnApi* | [**DeleteMsgVpnAclProfile**](docs/MsgVpnApi.md#deletemsgvpnaclprofile) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName} | Delete an ACL Profile object.
*MsgVpnApi* | [**DeleteMsgVpnAclProfileClientConnectException**](docs/MsgVpnApi.md#deletemsgvpnaclprofileclientconnectexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions/{clientConnectExceptionAddress} | Delete a Client Connect Exception object.
*MsgVpnApi* | [**DeleteMsgVpnAclProfilePublishException**](docs/MsgVpnApi.md#deletemsgvpnaclprofilepublishexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions/{topicSyntax},{publishExceptionTopic} | Delete a Publish Topic Exception object.
*MsgVpnApi* | [**DeleteMsgVpnAclProfilePublishTopicException**](docs/MsgVpnApi.md#deletemsgvpnaclprofilepublishtopicexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions/{publishTopicExceptionSyntax},{publishTopicException} | Delete a Publish Topic Exception object.
*MsgVpnApi* | [**DeleteMsgVpnAclProfileSubscribeException**](docs/MsgVpnApi.md#deletemsgvpnaclprofilesubscribeexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions/{topicSyntax},{subscribeExceptionTopic} | Delete a Subscribe Topic Exception object.
*MsgVpnApi* | [**DeleteMsgVpnAclProfileSubscribeShareNameException**](docs/MsgVpnApi.md#deletemsgvpnaclprofilesubscribesharenameexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions/{subscribeShareNameExceptionSyntax},{subscribeShareNameException} | Delete a Subscribe Share Name Exception object.
*MsgVpnApi* | [**DeleteMsgVpnAclProfileSubscribeTopicException**](docs/MsgVpnApi.md#deletemsgvpnaclprofilesubscribetopicexception) | **DELETE** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions/{subscribeTopicExceptionSyntax},{subscribeTopicException} | Delete a Subscribe Topic Exception object.
*MsgVpnApi* | [**DeleteMsgVpnAuthenticationOauthProfile**](docs/MsgVpnApi.md#deletemsgvpnauthenticationoauthprofile) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Delete an OAuth Profile object.
*MsgVpnApi* | [**DeleteMsgVpnAuthenticationOauthProfileClientRequiredClaim**](docs/MsgVpnApi.md#deletemsgvpnauthenticationoauthprofileclientrequiredclaim) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Delete a Required Claim object.
*MsgVpnApi* | [**DeleteMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**](docs/MsgVpnApi.md#deletemsgvpnauthenticationoauthprofileresourceserverrequiredclaim) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Delete a Required Claim object.
*MsgVpnApi* | [**DeleteMsgVpnAuthenticationOauthProvider**](docs/MsgVpnApi.md#deletemsgvpnauthenticationoauthprovider) | **DELETE** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Delete an OAuth Provider object.
*MsgVpnApi* | [**DeleteMsgVpnAuthorizationGroup**](docs/MsgVpnApi.md#deletemsgvpnauthorizationgroup) | **DELETE** /msgVpns/{msgVpnName}/authorizationGroups/{authorizationGroupName} | Delete an Authorization Group object.
*MsgVpnApi* | [**DeleteMsgVpnBridge**](docs/MsgVpnApi.md#deletemsgvpnbridge) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Delete a Bridge object.
*MsgVpnApi* | [**DeleteMsgVpnBridgeRemoteMsgVpn**](docs/MsgVpnApi.md#deletemsgvpnbridgeremotemsgvpn) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Delete a Remote Message VPN object.
*MsgVpnApi* | [**DeleteMsgVpnBridgeRemoteSubscription**](docs/MsgVpnApi.md#deletemsgvpnbridgeremotesubscription) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions/{remoteSubscriptionTopic} | Delete a Remote Subscription object.
*MsgVpnApi* | [**DeleteMsgVpnBridgeTlsTrustedCommonName**](docs/MsgVpnApi.md#deletemsgvpnbridgetlstrustedcommonname) | **DELETE** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Delete a Trusted Common Name object.
*MsgVpnApi* | [**DeleteMsgVpnCertMatchingRule**](docs/MsgVpnApi.md#deletemsgvpncertmatchingrule) | **DELETE** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Delete a Certificate Matching Rule object.
*MsgVpnApi* | [**DeleteMsgVpnCertMatchingRuleAttributeFilter**](docs/MsgVpnApi.md#deletemsgvpncertmatchingruleattributefilter) | **DELETE** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Delete a Certificate Matching Rule Attribute Filter object.
*MsgVpnApi* | [**DeleteMsgVpnCertMatchingRuleCondition**](docs/MsgVpnApi.md#deletemsgvpncertmatchingrulecondition) | **DELETE** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions/{source} | Delete a Certificate Matching Rule Condition object.
*MsgVpnApi* | [**DeleteMsgVpnClientProfile**](docs/MsgVpnApi.md#deletemsgvpnclientprofile) | **DELETE** /msgVpns/{msgVpnName}/clientProfiles/{clientProfileName} | Delete a Client Profile object.
*MsgVpnApi* | [**DeleteMsgVpnClientUsername**](docs/MsgVpnApi.md#deletemsgvpnclientusername) | **DELETE** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername} | Delete a Client Username object.
*MsgVpnApi* | [**DeleteMsgVpnClientUsernameAttribute**](docs/MsgVpnApi.md#deletemsgvpnclientusernameattribute) | **DELETE** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername}/attributes/{attributeName},{attributeValue} | Delete a Client Username Attribute object.
*MsgVpnApi* | [**DeleteMsgVpnDistributedCache**](docs/MsgVpnApi.md#deletemsgvpndistributedcache) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Delete a Distributed Cache object.
*MsgVpnApi* | [**DeleteMsgVpnDistributedCacheCluster**](docs/MsgVpnApi.md#deletemsgvpndistributedcachecluster) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Delete a Cache Cluster object.
*MsgVpnApi* | [**DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**](docs/MsgVpnApi.md#deletemsgvpndistributedcacheclusterglobalcachinghomecluster) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName} | Delete a Home Cache Cluster object.
*MsgVpnApi* | [**DeleteMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**](docs/MsgVpnApi.md#deletemsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes/{topicPrefix} | Delete a Topic Prefix object.
*MsgVpnApi* | [**DeleteMsgVpnDistributedCacheClusterInstance**](docs/MsgVpnApi.md#deletemsgvpndistributedcacheclusterinstance) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Delete a Cache Instance object.
*MsgVpnApi* | [**DeleteMsgVpnDistributedCacheClusterTopic**](docs/MsgVpnApi.md#deletemsgvpndistributedcacheclustertopic) | **DELETE** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics/{topic} | Delete a Topic object.
*MsgVpnApi* | [**DeleteMsgVpnDmrBridge**](docs/MsgVpnApi.md#deletemsgvpndmrbridge) | **DELETE** /msgVpns/{msgVpnName}/dmrBridges/{remoteNodeName} | Delete a DMR Bridge object.
*MsgVpnApi* | [**DeleteMsgVpnJndiConnectionFactory**](docs/MsgVpnApi.md#deletemsgvpnjndiconnectionfactory) | **DELETE** /msgVpns/{msgVpnName}/jndiConnectionFactories/{connectionFactoryName} | Delete a JNDI Connection Factory object.
*MsgVpnApi* | [**DeleteMsgVpnJndiQueue**](docs/MsgVpnApi.md#deletemsgvpnjndiqueue) | **DELETE** /msgVpns/{msgVpnName}/jndiQueues/{queueName} | Delete a JNDI Queue object.
*MsgVpnApi* | [**DeleteMsgVpnJndiTopic**](docs/MsgVpnApi.md#deletemsgvpnjnditopic) | **DELETE** /msgVpns/{msgVpnName}/jndiTopics/{topicName} | Delete a JNDI Topic object.
*MsgVpnApi* | [**DeleteMsgVpnMqttRetainCache**](docs/MsgVpnApi.md#deletemsgvpnmqttretaincache) | **DELETE** /msgVpns/{msgVpnName}/mqttRetainCaches/{cacheName} | Delete an MQTT Retain Cache object.
*MsgVpnApi* | [**DeleteMsgVpnMqttSession**](docs/MsgVpnApi.md#deletemsgvpnmqttsession) | **DELETE** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter} | Delete an MQTT Session object.
*MsgVpnApi* | [**DeleteMsgVpnMqttSessionSubscription**](docs/MsgVpnApi.md#deletemsgvpnmqttsessionsubscription) | **DELETE** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions/{subscriptionTopic} | Delete a Subscription object.
*MsgVpnApi* | [**DeleteMsgVpnQueue**](docs/MsgVpnApi.md#deletemsgvpnqueue) | **DELETE** /msgVpns/{msgVpnName}/queues/{queueName} | Delete a Queue object.
*MsgVpnApi* | [**DeleteMsgVpnQueueSubscription**](docs/MsgVpnApi.md#deletemsgvpnqueuesubscription) | **DELETE** /msgVpns/{msgVpnName}/queues/{queueName}/subscriptions/{subscriptionTopic} | Delete a Queue Subscription object.
*MsgVpnApi* | [**DeleteMsgVpnQueueTemplate**](docs/MsgVpnApi.md#deletemsgvpnqueuetemplate) | **DELETE** /msgVpns/{msgVpnName}/queueTemplates/{queueTemplateName} | Delete a Queue Template object.
*MsgVpnApi* | [**DeleteMsgVpnReplayLog**](docs/MsgVpnApi.md#deletemsgvpnreplaylog) | **DELETE** /msgVpns/{msgVpnName}/replayLogs/{replayLogName} | Delete a Replay Log object.
*MsgVpnApi* | [**DeleteMsgVpnReplayLogTopicFilterSubscription**](docs/MsgVpnApi.md#deletemsgvpnreplaylogtopicfiltersubscription) | **DELETE** /msgVpns/{msgVpnName}/replayLogs/{replayLogName}/topicFilterSubscriptions/{topicFilterSubscription} | Delete a Topic Filter Subscription object.
*MsgVpnApi* | [**DeleteMsgVpnReplicatedTopic**](docs/MsgVpnApi.md#deletemsgvpnreplicatedtopic) | **DELETE** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Delete a Replicated Topic object.
*MsgVpnApi* | [**DeleteMsgVpnRestDeliveryPoint**](docs/MsgVpnApi.md#deletemsgvpnrestdeliverypoint) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Delete a REST Delivery Point object.
*MsgVpnApi* | [**DeleteMsgVpnRestDeliveryPointQueueBinding**](docs/MsgVpnApi.md#deletemsgvpnrestdeliverypointqueuebinding) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Delete a Queue Binding object.
*MsgVpnApi* | [**DeleteMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/MsgVpnApi.md#deletemsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Delete a Protected Request Header object.
*MsgVpnApi* | [**DeleteMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/MsgVpnApi.md#deletemsgvpnrestdeliverypointqueuebindingrequestheader) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Delete a Request Header object.
*MsgVpnApi* | [**DeleteMsgVpnRestDeliveryPointRestConsumer**](docs/MsgVpnApi.md#deletemsgvpnrestdeliverypointrestconsumer) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Delete a REST Consumer object.
*MsgVpnApi* | [**DeleteMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**](docs/MsgVpnApi.md#deletemsgvpnrestdeliverypointrestconsumeroauthjwtclaim) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims/{oauthJwtClaimName} | Delete a Claim object.
*MsgVpnApi* | [**DeleteMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**](docs/MsgVpnApi.md#deletemsgvpnrestdeliverypointrestconsumertlstrustedcommonname) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Delete a Trusted Common Name object.
*MsgVpnApi* | [**DeleteMsgVpnSequencedTopic**](docs/MsgVpnApi.md#deletemsgvpnsequencedtopic) | **DELETE** /msgVpns/{msgVpnName}/sequencedTopics/{sequencedTopic} | Delete a Sequenced Topic object.
*MsgVpnApi* | [**DeleteMsgVpnTelemetryProfile**](docs/MsgVpnApi.md#deletemsgvpntelemetryprofile) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Delete a Telemetry Profile object.
*MsgVpnApi* | [**DeleteMsgVpnTelemetryProfileReceiverAclConnectException**](docs/MsgVpnApi.md#deletemsgvpntelemetryprofilereceiveraclconnectexception) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions/{receiverAclConnectExceptionAddress} | Delete a Receiver ACL Connect Exception object.
*MsgVpnApi* | [**DeleteMsgVpnTelemetryProfileTraceFilter**](docs/MsgVpnApi.md#deletemsgvpntelemetryprofiletracefilter) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Delete a Trace Filter object.
*MsgVpnApi* | [**DeleteMsgVpnTelemetryProfileTraceFilterSubscription**](docs/MsgVpnApi.md#deletemsgvpntelemetryprofiletracefiltersubscription) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions/{subscription},{subscriptionSyntax} | Delete a Telemetry Trace Filter Subscription object.
*MsgVpnApi* | [**DeleteMsgVpnTopicEndpoint**](docs/MsgVpnApi.md#deletemsgvpntopicendpoint) | **DELETE** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Delete a Topic Endpoint object.
*MsgVpnApi* | [**DeleteMsgVpnTopicEndpointTemplate**](docs/MsgVpnApi.md#deletemsgvpntopicendpointtemplate) | **DELETE** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Delete a Topic Endpoint Template object.
*MsgVpnApi* | [**GetMsgVpn**](docs/MsgVpnApi.md#getmsgvpn) | **GET** /msgVpns/{msgVpnName} | Get a Message VPN object.
*MsgVpnApi* | [**GetMsgVpnAclProfile**](docs/MsgVpnApi.md#getmsgvpnaclprofile) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName} | Get an ACL Profile object.
*MsgVpnApi* | [**GetMsgVpnAclProfileClientConnectException**](docs/MsgVpnApi.md#getmsgvpnaclprofileclientconnectexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions/{clientConnectExceptionAddress} | Get a Client Connect Exception object.
*MsgVpnApi* | [**GetMsgVpnAclProfileClientConnectExceptions**](docs/MsgVpnApi.md#getmsgvpnaclprofileclientconnectexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions | Get a list of Client Connect Exception objects.
*MsgVpnApi* | [**GetMsgVpnAclProfilePublishException**](docs/MsgVpnApi.md#getmsgvpnaclprofilepublishexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions/{topicSyntax},{publishExceptionTopic} | Get a Publish Topic Exception object.
*MsgVpnApi* | [**GetMsgVpnAclProfilePublishExceptions**](docs/MsgVpnApi.md#getmsgvpnaclprofilepublishexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions | Get a list of Publish Topic Exception objects.
*MsgVpnApi* | [**GetMsgVpnAclProfilePublishTopicException**](docs/MsgVpnApi.md#getmsgvpnaclprofilepublishtopicexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions/{publishTopicExceptionSyntax},{publishTopicException} | Get a Publish Topic Exception object.
*MsgVpnApi* | [**GetMsgVpnAclProfilePublishTopicExceptions**](docs/MsgVpnApi.md#getmsgvpnaclprofilepublishtopicexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions | Get a list of Publish Topic Exception objects.
*MsgVpnApi* | [**GetMsgVpnAclProfileSubscribeException**](docs/MsgVpnApi.md#getmsgvpnaclprofilesubscribeexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions/{topicSyntax},{subscribeExceptionTopic} | Get a Subscribe Topic Exception object.
*MsgVpnApi* | [**GetMsgVpnAclProfileSubscribeExceptions**](docs/MsgVpnApi.md#getmsgvpnaclprofilesubscribeexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions | Get a list of Subscribe Topic Exception objects.
*MsgVpnApi* | [**GetMsgVpnAclProfileSubscribeShareNameException**](docs/MsgVpnApi.md#getmsgvpnaclprofilesubscribesharenameexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions/{subscribeShareNameExceptionSyntax},{subscribeShareNameException} | Get a Subscribe Share Name Exception object.
*MsgVpnApi* | [**GetMsgVpnAclProfileSubscribeShareNameExceptions**](docs/MsgVpnApi.md#getmsgvpnaclprofilesubscribesharenameexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions | Get a list of Subscribe Share Name Exception objects.
*MsgVpnApi* | [**GetMsgVpnAclProfileSubscribeTopicException**](docs/MsgVpnApi.md#getmsgvpnaclprofilesubscribetopicexception) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions/{subscribeTopicExceptionSyntax},{subscribeTopicException} | Get a Subscribe Topic Exception object.
*MsgVpnApi* | [**GetMsgVpnAclProfileSubscribeTopicExceptions**](docs/MsgVpnApi.md#getmsgvpnaclprofilesubscribetopicexceptions) | **GET** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions | Get a list of Subscribe Topic Exception objects.
*MsgVpnApi* | [**GetMsgVpnAclProfiles**](docs/MsgVpnApi.md#getmsgvpnaclprofiles) | **GET** /msgVpns/{msgVpnName}/aclProfiles | Get a list of ACL Profile objects.
*MsgVpnApi* | [**GetMsgVpnAuthenticationOauthProfile**](docs/MsgVpnApi.md#getmsgvpnauthenticationoauthprofile) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Get an OAuth Profile object.
*MsgVpnApi* | [**GetMsgVpnAuthenticationOauthProfileClientRequiredClaim**](docs/MsgVpnApi.md#getmsgvpnauthenticationoauthprofileclientrequiredclaim) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Get a Required Claim object.
*MsgVpnApi* | [**GetMsgVpnAuthenticationOauthProfileClientRequiredClaims**](docs/MsgVpnApi.md#getmsgvpnauthenticationoauthprofileclientrequiredclaims) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/clientRequiredClaims | Get a list of Required Claim objects.
*MsgVpnApi* | [**GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaim**](docs/MsgVpnApi.md#getmsgvpnauthenticationoauthprofileresourceserverrequiredclaim) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Get a Required Claim object.
*MsgVpnApi* | [**GetMsgVpnAuthenticationOauthProfileResourceServerRequiredClaims**](docs/MsgVpnApi.md#getmsgvpnauthenticationoauthprofileresourceserverrequiredclaims) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Get a list of Required Claim objects.
*MsgVpnApi* | [**GetMsgVpnAuthenticationOauthProfiles**](docs/MsgVpnApi.md#getmsgvpnauthenticationoauthprofiles) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProfiles | Get a list of OAuth Profile objects.
*MsgVpnApi* | [**GetMsgVpnAuthenticationOauthProvider**](docs/MsgVpnApi.md#getmsgvpnauthenticationoauthprovider) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Get an OAuth Provider object.
*MsgVpnApi* | [**GetMsgVpnAuthenticationOauthProviders**](docs/MsgVpnApi.md#getmsgvpnauthenticationoauthproviders) | **GET** /msgVpns/{msgVpnName}/authenticationOauthProviders | Get a list of OAuth Provider objects.
*MsgVpnApi* | [**GetMsgVpnAuthorizationGroup**](docs/MsgVpnApi.md#getmsgvpnauthorizationgroup) | **GET** /msgVpns/{msgVpnName}/authorizationGroups/{authorizationGroupName} | Get an Authorization Group object.
*MsgVpnApi* | [**GetMsgVpnAuthorizationGroups**](docs/MsgVpnApi.md#getmsgvpnauthorizationgroups) | **GET** /msgVpns/{msgVpnName}/authorizationGroups | Get a list of Authorization Group objects.
*MsgVpnApi* | [**GetMsgVpnBridge**](docs/MsgVpnApi.md#getmsgvpnbridge) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Get a Bridge object.
*MsgVpnApi* | [**GetMsgVpnBridgeRemoteMsgVpn**](docs/MsgVpnApi.md#getmsgvpnbridgeremotemsgvpn) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Get a Remote Message VPN object.
*MsgVpnApi* | [**GetMsgVpnBridgeRemoteMsgVpns**](docs/MsgVpnApi.md#getmsgvpnbridgeremotemsgvpns) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns | Get a list of Remote Message VPN objects.
*MsgVpnApi* | [**GetMsgVpnBridgeRemoteSubscription**](docs/MsgVpnApi.md#getmsgvpnbridgeremotesubscription) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions/{remoteSubscriptionTopic} | Get a Remote Subscription object.
*MsgVpnApi* | [**GetMsgVpnBridgeRemoteSubscriptions**](docs/MsgVpnApi.md#getmsgvpnbridgeremotesubscriptions) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteSubscriptions | Get a list of Remote Subscription objects.
*MsgVpnApi* | [**GetMsgVpnBridgeTlsTrustedCommonName**](docs/MsgVpnApi.md#getmsgvpnbridgetlstrustedcommonname) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Get a Trusted Common Name object.
*MsgVpnApi* | [**GetMsgVpnBridgeTlsTrustedCommonNames**](docs/MsgVpnApi.md#getmsgvpnbridgetlstrustedcommonnames) | **GET** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/tlsTrustedCommonNames | Get a list of Trusted Common Name objects.
*MsgVpnApi* | [**GetMsgVpnBridges**](docs/MsgVpnApi.md#getmsgvpnbridges) | **GET** /msgVpns/{msgVpnName}/bridges | Get a list of Bridge objects.
*MsgVpnApi* | [**GetMsgVpnCertMatchingRule**](docs/MsgVpnApi.md#getmsgvpncertmatchingrule) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Get a Certificate Matching Rule object.
*MsgVpnApi* | [**GetMsgVpnCertMatchingRuleAttributeFilter**](docs/MsgVpnApi.md#getmsgvpncertmatchingruleattributefilter) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Get a Certificate Matching Rule Attribute Filter object.
*MsgVpnApi* | [**GetMsgVpnCertMatchingRuleAttributeFilters**](docs/MsgVpnApi.md#getmsgvpncertmatchingruleattributefilters) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters | Get a list of Certificate Matching Rule Attribute Filter objects.
*MsgVpnApi* | [**GetMsgVpnCertMatchingRuleCondition**](docs/MsgVpnApi.md#getmsgvpncertmatchingrulecondition) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions/{source} | Get a Certificate Matching Rule Condition object.
*MsgVpnApi* | [**GetMsgVpnCertMatchingRuleConditions**](docs/MsgVpnApi.md#getmsgvpncertmatchingruleconditions) | **GET** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/conditions | Get a list of Certificate Matching Rule Condition objects.
*MsgVpnApi* | [**GetMsgVpnCertMatchingRules**](docs/MsgVpnApi.md#getmsgvpncertmatchingrules) | **GET** /msgVpns/{msgVpnName}/certMatchingRules | Get a list of Certificate Matching Rule objects.
*MsgVpnApi* | [**GetMsgVpnClientProfile**](docs/MsgVpnApi.md#getmsgvpnclientprofile) | **GET** /msgVpns/{msgVpnName}/clientProfiles/{clientProfileName} | Get a Client Profile object.
*MsgVpnApi* | [**GetMsgVpnClientProfiles**](docs/MsgVpnApi.md#getmsgvpnclientprofiles) | **GET** /msgVpns/{msgVpnName}/clientProfiles | Get a list of Client Profile objects.
*MsgVpnApi* | [**GetMsgVpnClientUsername**](docs/MsgVpnApi.md#getmsgvpnclientusername) | **GET** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername} | Get a Client Username object.
*MsgVpnApi* | [**GetMsgVpnClientUsernameAttribute**](docs/MsgVpnApi.md#getmsgvpnclientusernameattribute) | **GET** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername}/attributes/{attributeName},{attributeValue} | Get a Client Username Attribute object.
*MsgVpnApi* | [**GetMsgVpnClientUsernameAttributes**](docs/MsgVpnApi.md#getmsgvpnclientusernameattributes) | **GET** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername}/attributes | Get a list of Client Username Attribute objects.
*MsgVpnApi* | [**GetMsgVpnClientUsernames**](docs/MsgVpnApi.md#getmsgvpnclientusernames) | **GET** /msgVpns/{msgVpnName}/clientUsernames | Get a list of Client Username objects.
*MsgVpnApi* | [**GetMsgVpnDistributedCache**](docs/MsgVpnApi.md#getmsgvpndistributedcache) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Get a Distributed Cache object.
*MsgVpnApi* | [**GetMsgVpnDistributedCacheCluster**](docs/MsgVpnApi.md#getmsgvpndistributedcachecluster) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Get a Cache Cluster object.
*MsgVpnApi* | [**GetMsgVpnDistributedCacheClusterGlobalCachingHomeCluster**](docs/MsgVpnApi.md#getmsgvpndistributedcacheclusterglobalcachinghomecluster) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName} | Get a Home Cache Cluster object.
*MsgVpnApi* | [**GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix**](docs/MsgVpnApi.md#getmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefix) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes/{topicPrefix} | Get a Topic Prefix object.
*MsgVpnApi* | [**GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixes**](docs/MsgVpnApi.md#getmsgvpndistributedcacheclusterglobalcachinghomeclustertopicprefixes) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters/{homeClusterName}/topicPrefixes | Get a list of Topic Prefix objects.
*MsgVpnApi* | [**GetMsgVpnDistributedCacheClusterGlobalCachingHomeClusters**](docs/MsgVpnApi.md#getmsgvpndistributedcacheclusterglobalcachinghomeclusters) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/globalCachingHomeClusters | Get a list of Home Cache Cluster objects.
*MsgVpnApi* | [**GetMsgVpnDistributedCacheClusterInstance**](docs/MsgVpnApi.md#getmsgvpndistributedcacheclusterinstance) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Get a Cache Instance object.
*MsgVpnApi* | [**GetMsgVpnDistributedCacheClusterInstances**](docs/MsgVpnApi.md#getmsgvpndistributedcacheclusterinstances) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances | Get a list of Cache Instance objects.
*MsgVpnApi* | [**GetMsgVpnDistributedCacheClusterTopic**](docs/MsgVpnApi.md#getmsgvpndistributedcacheclustertopic) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics/{topic} | Get a Topic object.
*MsgVpnApi* | [**GetMsgVpnDistributedCacheClusterTopics**](docs/MsgVpnApi.md#getmsgvpndistributedcacheclustertopics) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/topics | Get a list of Topic objects.
*MsgVpnApi* | [**GetMsgVpnDistributedCacheClusters**](docs/MsgVpnApi.md#getmsgvpndistributedcacheclusters) | **GET** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters | Get a list of Cache Cluster objects.
*MsgVpnApi* | [**GetMsgVpnDistributedCaches**](docs/MsgVpnApi.md#getmsgvpndistributedcaches) | **GET** /msgVpns/{msgVpnName}/distributedCaches | Get a list of Distributed Cache objects.
*MsgVpnApi* | [**GetMsgVpnDmrBridge**](docs/MsgVpnApi.md#getmsgvpndmrbridge) | **GET** /msgVpns/{msgVpnName}/dmrBridges/{remoteNodeName} | Get a DMR Bridge object.
*MsgVpnApi* | [**GetMsgVpnDmrBridges**](docs/MsgVpnApi.md#getmsgvpndmrbridges) | **GET** /msgVpns/{msgVpnName}/dmrBridges | Get a list of DMR Bridge objects.
*MsgVpnApi* | [**GetMsgVpnJndiConnectionFactories**](docs/MsgVpnApi.md#getmsgvpnjndiconnectionfactories) | **GET** /msgVpns/{msgVpnName}/jndiConnectionFactories | Get a list of JNDI Connection Factory objects.
*MsgVpnApi* | [**GetMsgVpnJndiConnectionFactory**](docs/MsgVpnApi.md#getmsgvpnjndiconnectionfactory) | **GET** /msgVpns/{msgVpnName}/jndiConnectionFactories/{connectionFactoryName} | Get a JNDI Connection Factory object.
*MsgVpnApi* | [**GetMsgVpnJndiQueue**](docs/MsgVpnApi.md#getmsgvpnjndiqueue) | **GET** /msgVpns/{msgVpnName}/jndiQueues/{queueName} | Get a JNDI Queue object.
*MsgVpnApi* | [**GetMsgVpnJndiQueues**](docs/MsgVpnApi.md#getmsgvpnjndiqueues) | **GET** /msgVpns/{msgVpnName}/jndiQueues | Get a list of JNDI Queue objects.
*MsgVpnApi* | [**GetMsgVpnJndiTopic**](docs/MsgVpnApi.md#getmsgvpnjnditopic) | **GET** /msgVpns/{msgVpnName}/jndiTopics/{topicName} | Get a JNDI Topic object.
*MsgVpnApi* | [**GetMsgVpnJndiTopics**](docs/MsgVpnApi.md#getmsgvpnjnditopics) | **GET** /msgVpns/{msgVpnName}/jndiTopics | Get a list of JNDI Topic objects.
*MsgVpnApi* | [**GetMsgVpnMqttRetainCache**](docs/MsgVpnApi.md#getmsgvpnmqttretaincache) | **GET** /msgVpns/{msgVpnName}/mqttRetainCaches/{cacheName} | Get an MQTT Retain Cache object.
*MsgVpnApi* | [**GetMsgVpnMqttRetainCaches**](docs/MsgVpnApi.md#getmsgvpnmqttretaincaches) | **GET** /msgVpns/{msgVpnName}/mqttRetainCaches | Get a list of MQTT Retain Cache objects.
*MsgVpnApi* | [**GetMsgVpnMqttSession**](docs/MsgVpnApi.md#getmsgvpnmqttsession) | **GET** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter} | Get an MQTT Session object.
*MsgVpnApi* | [**GetMsgVpnMqttSessionSubscription**](docs/MsgVpnApi.md#getmsgvpnmqttsessionsubscription) | **GET** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions/{subscriptionTopic} | Get a Subscription object.
*MsgVpnApi* | [**GetMsgVpnMqttSessionSubscriptions**](docs/MsgVpnApi.md#getmsgvpnmqttsessionsubscriptions) | **GET** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions | Get a list of Subscription objects.
*MsgVpnApi* | [**GetMsgVpnMqttSessions**](docs/MsgVpnApi.md#getmsgvpnmqttsessions) | **GET** /msgVpns/{msgVpnName}/mqttSessions | Get a list of MQTT Session objects.
*MsgVpnApi* | [**GetMsgVpnQueue**](docs/MsgVpnApi.md#getmsgvpnqueue) | **GET** /msgVpns/{msgVpnName}/queues/{queueName} | Get a Queue object.
*MsgVpnApi* | [**GetMsgVpnQueueSubscription**](docs/MsgVpnApi.md#getmsgvpnqueuesubscription) | **GET** /msgVpns/{msgVpnName}/queues/{queueName}/subscriptions/{subscriptionTopic} | Get a Queue Subscription object.
*MsgVpnApi* | [**GetMsgVpnQueueSubscriptions**](docs/MsgVpnApi.md#getmsgvpnqueuesubscriptions) | **GET** /msgVpns/{msgVpnName}/queues/{queueName}/subscriptions | Get a list of Queue Subscription objects.
*MsgVpnApi* | [**GetMsgVpnQueueTemplate**](docs/MsgVpnApi.md#getmsgvpnqueuetemplate) | **GET** /msgVpns/{msgVpnName}/queueTemplates/{queueTemplateName} | Get a Queue Template object.
*MsgVpnApi* | [**GetMsgVpnQueueTemplates**](docs/MsgVpnApi.md#getmsgvpnqueuetemplates) | **GET** /msgVpns/{msgVpnName}/queueTemplates | Get a list of Queue Template objects.
*MsgVpnApi* | [**GetMsgVpnQueues**](docs/MsgVpnApi.md#getmsgvpnqueues) | **GET** /msgVpns/{msgVpnName}/queues | Get a list of Queue objects.
*MsgVpnApi* | [**GetMsgVpnReplayLog**](docs/MsgVpnApi.md#getmsgvpnreplaylog) | **GET** /msgVpns/{msgVpnName}/replayLogs/{replayLogName} | Get a Replay Log object.
*MsgVpnApi* | [**GetMsgVpnReplayLogTopicFilterSubscription**](docs/MsgVpnApi.md#getmsgvpnreplaylogtopicfiltersubscription) | **GET** /msgVpns/{msgVpnName}/replayLogs/{replayLogName}/topicFilterSubscriptions/{topicFilterSubscription} | Get a Topic Filter Subscription object.
*MsgVpnApi* | [**GetMsgVpnReplayLogTopicFilterSubscriptions**](docs/MsgVpnApi.md#getmsgvpnreplaylogtopicfiltersubscriptions) | **GET** /msgVpns/{msgVpnName}/replayLogs/{replayLogName}/topicFilterSubscriptions | Get a list of Topic Filter Subscription objects.
*MsgVpnApi* | [**GetMsgVpnReplayLogs**](docs/MsgVpnApi.md#getmsgvpnreplaylogs) | **GET** /msgVpns/{msgVpnName}/replayLogs | Get a list of Replay Log objects.
*MsgVpnApi* | [**GetMsgVpnReplicatedTopic**](docs/MsgVpnApi.md#getmsgvpnreplicatedtopic) | **GET** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Get a Replicated Topic object.
*MsgVpnApi* | [**GetMsgVpnReplicatedTopics**](docs/MsgVpnApi.md#getmsgvpnreplicatedtopics) | **GET** /msgVpns/{msgVpnName}/replicatedTopics | Get a list of Replicated Topic objects.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPoint**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypoint) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Get a REST Delivery Point object.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPointQueueBinding**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypointqueuebinding) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Get a Queue Binding object.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Get a Protected Request Header object.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaders**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypointqueuebindingprotectedrequestheaders) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders | Get a list of Protected Request Header objects.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypointqueuebindingrequestheader) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Get a Request Header object.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPointQueueBindingRequestHeaders**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypointqueuebindingrequestheaders) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders | Get a list of Request Header objects.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPointQueueBindings**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypointqueuebindings) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings | Get a list of Queue Binding objects.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPointRestConsumer**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypointrestconsumer) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Get a REST Consumer object.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypointrestconsumeroauthjwtclaim) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims/{oauthJwtClaimName} | Get a Claim object.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaims**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypointrestconsumeroauthjwtclaims) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims | Get a list of Claim objects.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypointrestconsumertlstrustedcommonname) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Get a Trusted Common Name object.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNames**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypointrestconsumertlstrustedcommonnames) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames | Get a list of Trusted Common Name objects.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPointRestConsumers**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypointrestconsumers) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers | Get a list of REST Consumer objects.
*MsgVpnApi* | [**GetMsgVpnRestDeliveryPoints**](docs/MsgVpnApi.md#getmsgvpnrestdeliverypoints) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints | Get a list of REST Delivery Point objects.
*MsgVpnApi* | [**GetMsgVpnSequencedTopic**](docs/MsgVpnApi.md#getmsgvpnsequencedtopic) | **GET** /msgVpns/{msgVpnName}/sequencedTopics/{sequencedTopic} | Get a Sequenced Topic object.
*MsgVpnApi* | [**GetMsgVpnSequencedTopics**](docs/MsgVpnApi.md#getmsgvpnsequencedtopics) | **GET** /msgVpns/{msgVpnName}/sequencedTopics | Get a list of Sequenced Topic objects.
*MsgVpnApi* | [**GetMsgVpnTelemetryProfile**](docs/MsgVpnApi.md#getmsgvpntelemetryprofile) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Get a Telemetry Profile object.
*MsgVpnApi* | [**GetMsgVpnTelemetryProfileReceiverAclConnectException**](docs/MsgVpnApi.md#getmsgvpntelemetryprofilereceiveraclconnectexception) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions/{receiverAclConnectExceptionAddress} | Get a Receiver ACL Connect Exception object.
*MsgVpnApi* | [**GetMsgVpnTelemetryProfileReceiverAclConnectExceptions**](docs/MsgVpnApi.md#getmsgvpntelemetryprofilereceiveraclconnectexceptions) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions | Get a list of Receiver ACL Connect Exception objects.
*MsgVpnApi* | [**GetMsgVpnTelemetryProfileTraceFilter**](docs/MsgVpnApi.md#getmsgvpntelemetryprofiletracefilter) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Get a Trace Filter object.
*MsgVpnApi* | [**GetMsgVpnTelemetryProfileTraceFilterSubscription**](docs/MsgVpnApi.md#getmsgvpntelemetryprofiletracefiltersubscription) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions/{subscription},{subscriptionSyntax} | Get a Telemetry Trace Filter Subscription object.
*MsgVpnApi* | [**GetMsgVpnTelemetryProfileTraceFilterSubscriptions**](docs/MsgVpnApi.md#getmsgvpntelemetryprofiletracefiltersubscriptions) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions | Get a list of Telemetry Trace Filter Subscription objects.
*MsgVpnApi* | [**GetMsgVpnTelemetryProfileTraceFilters**](docs/MsgVpnApi.md#getmsgvpntelemetryprofiletracefilters) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters | Get a list of Trace Filter objects.
*MsgVpnApi* | [**GetMsgVpnTelemetryProfiles**](docs/MsgVpnApi.md#getmsgvpntelemetryprofiles) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles | Get a list of Telemetry Profile objects.
*MsgVpnApi* | [**GetMsgVpnTopicEndpoint**](docs/MsgVpnApi.md#getmsgvpntopicendpoint) | **GET** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Get a Topic Endpoint object.
*MsgVpnApi* | [**GetMsgVpnTopicEndpointTemplate**](docs/MsgVpnApi.md#getmsgvpntopicendpointtemplate) | **GET** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Get a Topic Endpoint Template object.
*MsgVpnApi* | [**GetMsgVpnTopicEndpointTemplates**](docs/MsgVpnApi.md#getmsgvpntopicendpointtemplates) | **GET** /msgVpns/{msgVpnName}/topicEndpointTemplates | Get a list of Topic Endpoint Template objects.
*MsgVpnApi* | [**GetMsgVpnTopicEndpoints**](docs/MsgVpnApi.md#getmsgvpntopicendpoints) | **GET** /msgVpns/{msgVpnName}/topicEndpoints | Get a list of Topic Endpoint objects.
*MsgVpnApi* | [**GetMsgVpns**](docs/MsgVpnApi.md#getmsgvpns) | **GET** /msgVpns | Get a list of Message VPN objects.
*MsgVpnApi* | [**ReplaceMsgVpn**](docs/MsgVpnApi.md#replacemsgvpn) | **PUT** /msgVpns/{msgVpnName} | Replace a Message VPN object.
*MsgVpnApi* | [**ReplaceMsgVpnAclProfile**](docs/MsgVpnApi.md#replacemsgvpnaclprofile) | **PUT** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName} | Replace an ACL Profile object.
*MsgVpnApi* | [**ReplaceMsgVpnAuthenticationOauthProfile**](docs/MsgVpnApi.md#replacemsgvpnauthenticationoauthprofile) | **PUT** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Replace an OAuth Profile object.
*MsgVpnApi* | [**ReplaceMsgVpnAuthenticationOauthProvider**](docs/MsgVpnApi.md#replacemsgvpnauthenticationoauthprovider) | **PUT** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Replace an OAuth Provider object.
*MsgVpnApi* | [**ReplaceMsgVpnAuthorizationGroup**](docs/MsgVpnApi.md#replacemsgvpnauthorizationgroup) | **PUT** /msgVpns/{msgVpnName}/authorizationGroups/{authorizationGroupName} | Replace an Authorization Group object.
*MsgVpnApi* | [**ReplaceMsgVpnBridge**](docs/MsgVpnApi.md#replacemsgvpnbridge) | **PUT** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Replace a Bridge object.
*MsgVpnApi* | [**ReplaceMsgVpnBridgeRemoteMsgVpn**](docs/MsgVpnApi.md#replacemsgvpnbridgeremotemsgvpn) | **PUT** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Replace a Remote Message VPN object.
*MsgVpnApi* | [**ReplaceMsgVpnCertMatchingRule**](docs/MsgVpnApi.md#replacemsgvpncertmatchingrule) | **PUT** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Replace a Certificate Matching Rule object.
*MsgVpnApi* | [**ReplaceMsgVpnCertMatchingRuleAttributeFilter**](docs/MsgVpnApi.md#replacemsgvpncertmatchingruleattributefilter) | **PUT** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Replace a Certificate Matching Rule Attribute Filter object.
*MsgVpnApi* | [**ReplaceMsgVpnClientProfile**](docs/MsgVpnApi.md#replacemsgvpnclientprofile) | **PUT** /msgVpns/{msgVpnName}/clientProfiles/{clientProfileName} | Replace a Client Profile object.
*MsgVpnApi* | [**ReplaceMsgVpnClientUsername**](docs/MsgVpnApi.md#replacemsgvpnclientusername) | **PUT** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername} | Replace a Client Username object.
*MsgVpnApi* | [**ReplaceMsgVpnDistributedCache**](docs/MsgVpnApi.md#replacemsgvpndistributedcache) | **PUT** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Replace a Distributed Cache object.
*MsgVpnApi* | [**ReplaceMsgVpnDistributedCacheCluster**](docs/MsgVpnApi.md#replacemsgvpndistributedcachecluster) | **PUT** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Replace a Cache Cluster object.
*MsgVpnApi* | [**ReplaceMsgVpnDistributedCacheClusterInstance**](docs/MsgVpnApi.md#replacemsgvpndistributedcacheclusterinstance) | **PUT** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Replace a Cache Instance object.
*MsgVpnApi* | [**ReplaceMsgVpnDmrBridge**](docs/MsgVpnApi.md#replacemsgvpndmrbridge) | **PUT** /msgVpns/{msgVpnName}/dmrBridges/{remoteNodeName} | Replace a DMR Bridge object.
*MsgVpnApi* | [**ReplaceMsgVpnJndiConnectionFactory**](docs/MsgVpnApi.md#replacemsgvpnjndiconnectionfactory) | **PUT** /msgVpns/{msgVpnName}/jndiConnectionFactories/{connectionFactoryName} | Replace a JNDI Connection Factory object.
*MsgVpnApi* | [**ReplaceMsgVpnJndiQueue**](docs/MsgVpnApi.md#replacemsgvpnjndiqueue) | **PUT** /msgVpns/{msgVpnName}/jndiQueues/{queueName} | Replace a JNDI Queue object.
*MsgVpnApi* | [**ReplaceMsgVpnJndiTopic**](docs/MsgVpnApi.md#replacemsgvpnjnditopic) | **PUT** /msgVpns/{msgVpnName}/jndiTopics/{topicName} | Replace a JNDI Topic object.
*MsgVpnApi* | [**ReplaceMsgVpnMqttRetainCache**](docs/MsgVpnApi.md#replacemsgvpnmqttretaincache) | **PUT** /msgVpns/{msgVpnName}/mqttRetainCaches/{cacheName} | Replace an MQTT Retain Cache object.
*MsgVpnApi* | [**ReplaceMsgVpnMqttSession**](docs/MsgVpnApi.md#replacemsgvpnmqttsession) | **PUT** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter} | Replace an MQTT Session object.
*MsgVpnApi* | [**ReplaceMsgVpnMqttSessionSubscription**](docs/MsgVpnApi.md#replacemsgvpnmqttsessionsubscription) | **PUT** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions/{subscriptionTopic} | Replace a Subscription object.
*MsgVpnApi* | [**ReplaceMsgVpnQueue**](docs/MsgVpnApi.md#replacemsgvpnqueue) | **PUT** /msgVpns/{msgVpnName}/queues/{queueName} | Replace a Queue object.
*MsgVpnApi* | [**ReplaceMsgVpnQueueTemplate**](docs/MsgVpnApi.md#replacemsgvpnqueuetemplate) | **PUT** /msgVpns/{msgVpnName}/queueTemplates/{queueTemplateName} | Replace a Queue Template object.
*MsgVpnApi* | [**ReplaceMsgVpnReplayLog**](docs/MsgVpnApi.md#replacemsgvpnreplaylog) | **PUT** /msgVpns/{msgVpnName}/replayLogs/{replayLogName} | Replace a Replay Log object.
*MsgVpnApi* | [**ReplaceMsgVpnReplicatedTopic**](docs/MsgVpnApi.md#replacemsgvpnreplicatedtopic) | **PUT** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Replace a Replicated Topic object.
*MsgVpnApi* | [**ReplaceMsgVpnRestDeliveryPoint**](docs/MsgVpnApi.md#replacemsgvpnrestdeliverypoint) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Replace a REST Delivery Point object.
*MsgVpnApi* | [**ReplaceMsgVpnRestDeliveryPointQueueBinding**](docs/MsgVpnApi.md#replacemsgvpnrestdeliverypointqueuebinding) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Replace a Queue Binding object.
*MsgVpnApi* | [**ReplaceMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/MsgVpnApi.md#replacemsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Replace a Protected Request Header object.
*MsgVpnApi* | [**ReplaceMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/MsgVpnApi.md#replacemsgvpnrestdeliverypointqueuebindingrequestheader) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Replace a Request Header object.
*MsgVpnApi* | [**ReplaceMsgVpnRestDeliveryPointRestConsumer**](docs/MsgVpnApi.md#replacemsgvpnrestdeliverypointrestconsumer) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Replace a REST Consumer object.
*MsgVpnApi* | [**ReplaceMsgVpnTelemetryProfile**](docs/MsgVpnApi.md#replacemsgvpntelemetryprofile) | **PUT** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Replace a Telemetry Profile object.
*MsgVpnApi* | [**ReplaceMsgVpnTelemetryProfileTraceFilter**](docs/MsgVpnApi.md#replacemsgvpntelemetryprofiletracefilter) | **PUT** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Replace a Trace Filter object.
*MsgVpnApi* | [**ReplaceMsgVpnTopicEndpoint**](docs/MsgVpnApi.md#replacemsgvpntopicendpoint) | **PUT** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Replace a Topic Endpoint object.
*MsgVpnApi* | [**ReplaceMsgVpnTopicEndpointTemplate**](docs/MsgVpnApi.md#replacemsgvpntopicendpointtemplate) | **PUT** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Replace a Topic Endpoint Template object.
*MsgVpnApi* | [**UpdateMsgVpn**](docs/MsgVpnApi.md#updatemsgvpn) | **PATCH** /msgVpns/{msgVpnName} | Update a Message VPN object.
*MsgVpnApi* | [**UpdateMsgVpnAclProfile**](docs/MsgVpnApi.md#updatemsgvpnaclprofile) | **PATCH** /msgVpns/{msgVpnName}/aclProfiles/{aclProfileName} | Update an ACL Profile object.
*MsgVpnApi* | [**UpdateMsgVpnAuthenticationOauthProfile**](docs/MsgVpnApi.md#updatemsgvpnauthenticationoauthprofile) | **PATCH** /msgVpns/{msgVpnName}/authenticationOauthProfiles/{oauthProfileName} | Update an OAuth Profile object.
*MsgVpnApi* | [**UpdateMsgVpnAuthenticationOauthProvider**](docs/MsgVpnApi.md#updatemsgvpnauthenticationoauthprovider) | **PATCH** /msgVpns/{msgVpnName}/authenticationOauthProviders/{oauthProviderName} | Update an OAuth Provider object.
*MsgVpnApi* | [**UpdateMsgVpnAuthorizationGroup**](docs/MsgVpnApi.md#updatemsgvpnauthorizationgroup) | **PATCH** /msgVpns/{msgVpnName}/authorizationGroups/{authorizationGroupName} | Update an Authorization Group object.
*MsgVpnApi* | [**UpdateMsgVpnBridge**](docs/MsgVpnApi.md#updatemsgvpnbridge) | **PATCH** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter} | Update a Bridge object.
*MsgVpnApi* | [**UpdateMsgVpnBridgeRemoteMsgVpn**](docs/MsgVpnApi.md#updatemsgvpnbridgeremotemsgvpn) | **PATCH** /msgVpns/{msgVpnName}/bridges/{bridgeName},{bridgeVirtualRouter}/remoteMsgVpns/{remoteMsgVpnName},{remoteMsgVpnLocation},{remoteMsgVpnInterface} | Update a Remote Message VPN object.
*MsgVpnApi* | [**UpdateMsgVpnCertMatchingRule**](docs/MsgVpnApi.md#updatemsgvpncertmatchingrule) | **PATCH** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName} | Update a Certificate Matching Rule object.
*MsgVpnApi* | [**UpdateMsgVpnCertMatchingRuleAttributeFilter**](docs/MsgVpnApi.md#updatemsgvpncertmatchingruleattributefilter) | **PATCH** /msgVpns/{msgVpnName}/certMatchingRules/{ruleName}/attributeFilters/{filterName} | Update a Certificate Matching Rule Attribute Filter object.
*MsgVpnApi* | [**UpdateMsgVpnClientProfile**](docs/MsgVpnApi.md#updatemsgvpnclientprofile) | **PATCH** /msgVpns/{msgVpnName}/clientProfiles/{clientProfileName} | Update a Client Profile object.
*MsgVpnApi* | [**UpdateMsgVpnClientUsername**](docs/MsgVpnApi.md#updatemsgvpnclientusername) | **PATCH** /msgVpns/{msgVpnName}/clientUsernames/{clientUsername} | Update a Client Username object.
*MsgVpnApi* | [**UpdateMsgVpnDistributedCache**](docs/MsgVpnApi.md#updatemsgvpndistributedcache) | **PATCH** /msgVpns/{msgVpnName}/distributedCaches/{cacheName} | Update a Distributed Cache object.
*MsgVpnApi* | [**UpdateMsgVpnDistributedCacheCluster**](docs/MsgVpnApi.md#updatemsgvpndistributedcachecluster) | **PATCH** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName} | Update a Cache Cluster object.
*MsgVpnApi* | [**UpdateMsgVpnDistributedCacheClusterInstance**](docs/MsgVpnApi.md#updatemsgvpndistributedcacheclusterinstance) | **PATCH** /msgVpns/{msgVpnName}/distributedCaches/{cacheName}/clusters/{clusterName}/instances/{instanceName} | Update a Cache Instance object.
*MsgVpnApi* | [**UpdateMsgVpnDmrBridge**](docs/MsgVpnApi.md#updatemsgvpndmrbridge) | **PATCH** /msgVpns/{msgVpnName}/dmrBridges/{remoteNodeName} | Update a DMR Bridge object.
*MsgVpnApi* | [**UpdateMsgVpnJndiConnectionFactory**](docs/MsgVpnApi.md#updatemsgvpnjndiconnectionfactory) | **PATCH** /msgVpns/{msgVpnName}/jndiConnectionFactories/{connectionFactoryName} | Update a JNDI Connection Factory object.
*MsgVpnApi* | [**UpdateMsgVpnJndiQueue**](docs/MsgVpnApi.md#updatemsgvpnjndiqueue) | **PATCH** /msgVpns/{msgVpnName}/jndiQueues/{queueName} | Update a JNDI Queue object.
*MsgVpnApi* | [**UpdateMsgVpnJndiTopic**](docs/MsgVpnApi.md#updatemsgvpnjnditopic) | **PATCH** /msgVpns/{msgVpnName}/jndiTopics/{topicName} | Update a JNDI Topic object.
*MsgVpnApi* | [**UpdateMsgVpnMqttRetainCache**](docs/MsgVpnApi.md#updatemsgvpnmqttretaincache) | **PATCH** /msgVpns/{msgVpnName}/mqttRetainCaches/{cacheName} | Update an MQTT Retain Cache object.
*MsgVpnApi* | [**UpdateMsgVpnMqttSession**](docs/MsgVpnApi.md#updatemsgvpnmqttsession) | **PATCH** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter} | Update an MQTT Session object.
*MsgVpnApi* | [**UpdateMsgVpnMqttSessionSubscription**](docs/MsgVpnApi.md#updatemsgvpnmqttsessionsubscription) | **PATCH** /msgVpns/{msgVpnName}/mqttSessions/{mqttSessionClientId},{mqttSessionVirtualRouter}/subscriptions/{subscriptionTopic} | Update a Subscription object.
*MsgVpnApi* | [**UpdateMsgVpnQueue**](docs/MsgVpnApi.md#updatemsgvpnqueue) | **PATCH** /msgVpns/{msgVpnName}/queues/{queueName} | Update a Queue object.
*MsgVpnApi* | [**UpdateMsgVpnQueueTemplate**](docs/MsgVpnApi.md#updatemsgvpnqueuetemplate) | **PATCH** /msgVpns/{msgVpnName}/queueTemplates/{queueTemplateName} | Update a Queue Template object.
*MsgVpnApi* | [**UpdateMsgVpnReplayLog**](docs/MsgVpnApi.md#updatemsgvpnreplaylog) | **PATCH** /msgVpns/{msgVpnName}/replayLogs/{replayLogName} | Update a Replay Log object.
*MsgVpnApi* | [**UpdateMsgVpnReplicatedTopic**](docs/MsgVpnApi.md#updatemsgvpnreplicatedtopic) | **PATCH** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Update a Replicated Topic object.
*MsgVpnApi* | [**UpdateMsgVpnRestDeliveryPoint**](docs/MsgVpnApi.md#updatemsgvpnrestdeliverypoint) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Update a REST Delivery Point object.
*MsgVpnApi* | [**UpdateMsgVpnRestDeliveryPointQueueBinding**](docs/MsgVpnApi.md#updatemsgvpnrestdeliverypointqueuebinding) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Update a Queue Binding object.
*MsgVpnApi* | [**UpdateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/MsgVpnApi.md#updatemsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Update a Protected Request Header object.
*MsgVpnApi* | [**UpdateMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/MsgVpnApi.md#updatemsgvpnrestdeliverypointqueuebindingrequestheader) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Update a Request Header object.
*MsgVpnApi* | [**UpdateMsgVpnRestDeliveryPointRestConsumer**](docs/MsgVpnApi.md#updatemsgvpnrestdeliverypointrestconsumer) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Update a REST Consumer object.
*MsgVpnApi* | [**UpdateMsgVpnTelemetryProfile**](docs/MsgVpnApi.md#updatemsgvpntelemetryprofile) | **PATCH** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Update a Telemetry Profile object.
*MsgVpnApi* | [**UpdateMsgVpnTelemetryProfileTraceFilter**](docs/MsgVpnApi.md#updatemsgvpntelemetryprofiletracefilter) | **PATCH** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Update a Trace Filter object.
*MsgVpnApi* | [**UpdateMsgVpnTopicEndpoint**](docs/MsgVpnApi.md#updatemsgvpntopicendpoint) | **PATCH** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Update a Topic Endpoint object.
*MsgVpnApi* | [**UpdateMsgVpnTopicEndpointTemplate**](docs/MsgVpnApi.md#updatemsgvpntopicendpointtemplate) | **PATCH** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Update a Topic Endpoint Template object.
*OauthProfileApi* | [**CreateOauthProfile**](docs/OauthProfileApi.md#createoauthprofile) | **POST** /oauthProfiles | Create an OAuth Profile object.
*OauthProfileApi* | [**CreateOauthProfileAccessLevelGroup**](docs/OauthProfileApi.md#createoauthprofileaccesslevelgroup) | **POST** /oauthProfiles/{oauthProfileName}/accessLevelGroups | Create a Group Access Level object.
*OauthProfileApi* | [**CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](docs/OauthProfileApi.md#createoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **POST** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions | Create a Message VPN Access-Level Exception object.
*OauthProfileApi* | [**CreateOauthProfileClientAllowedHost**](docs/OauthProfileApi.md#createoauthprofileclientallowedhost) | **POST** /oauthProfiles/{oauthProfileName}/clientAllowedHosts | Create an Allowed Host Value object.
*OauthProfileApi* | [**CreateOauthProfileClientAuthorizationParameter**](docs/OauthProfileApi.md#createoauthprofileclientauthorizationparameter) | **POST** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters | Create an Authorization Parameter object.
*OauthProfileApi* | [**CreateOauthProfileClientRequiredClaim**](docs/OauthProfileApi.md#createoauthprofileclientrequiredclaim) | **POST** /oauthProfiles/{oauthProfileName}/clientRequiredClaims | Create a Required Claim object.
*OauthProfileApi* | [**CreateOauthProfileDefaultMsgVpnAccessLevelException**](docs/OauthProfileApi.md#createoauthprofiledefaultmsgvpnaccesslevelexception) | **POST** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions | Create a Message VPN Access-Level Exception object.
*OauthProfileApi* | [**CreateOauthProfileResourceServerRequiredClaim**](docs/OauthProfileApi.md#createoauthprofileresourceserverrequiredclaim) | **POST** /oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Create a Required Claim object.
*OauthProfileApi* | [**DeleteOauthProfile**](docs/OauthProfileApi.md#deleteoauthprofile) | **DELETE** /oauthProfiles/{oauthProfileName} | Delete an OAuth Profile object.
*OauthProfileApi* | [**DeleteOauthProfileAccessLevelGroup**](docs/OauthProfileApi.md#deleteoauthprofileaccesslevelgroup) | **DELETE** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName} | Delete a Group Access Level object.
*OauthProfileApi* | [**DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](docs/OauthProfileApi.md#deleteoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **DELETE** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName} | Delete a Message VPN Access-Level Exception object.
*OauthProfileApi* | [**DeleteOauthProfileClientAllowedHost**](docs/OauthProfileApi.md#deleteoauthprofileclientallowedhost) | **DELETE** /oauthProfiles/{oauthProfileName}/clientAllowedHosts/{allowedHost} | Delete an Allowed Host Value object.
*OauthProfileApi* | [**DeleteOauthProfileClientAuthorizationParameter**](docs/OauthProfileApi.md#deleteoauthprofileclientauthorizationparameter) | **DELETE** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName} | Delete an Authorization Parameter object.
*OauthProfileApi* | [**DeleteOauthProfileClientRequiredClaim**](docs/OauthProfileApi.md#deleteoauthprofileclientrequiredclaim) | **DELETE** /oauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Delete a Required Claim object.
*OauthProfileApi* | [**DeleteOauthProfileDefaultMsgVpnAccessLevelException**](docs/OauthProfileApi.md#deleteoauthprofiledefaultmsgvpnaccesslevelexception) | **DELETE** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName} | Delete a Message VPN Access-Level Exception object.
*OauthProfileApi* | [**DeleteOauthProfileResourceServerRequiredClaim**](docs/OauthProfileApi.md#deleteoauthprofileresourceserverrequiredclaim) | **DELETE** /oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Delete a Required Claim object.
*OauthProfileApi* | [**GetOauthProfile**](docs/OauthProfileApi.md#getoauthprofile) | **GET** /oauthProfiles/{oauthProfileName} | Get an OAuth Profile object.
*OauthProfileApi* | [**GetOauthProfileAccessLevelGroup**](docs/OauthProfileApi.md#getoauthprofileaccesslevelgroup) | **GET** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName} | Get a Group Access Level object.
*OauthProfileApi* | [**GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](docs/OauthProfileApi.md#getoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **GET** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName} | Get a Message VPN Access-Level Exception object.
*OauthProfileApi* | [**GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions**](docs/OauthProfileApi.md#getoauthprofileaccesslevelgroupmsgvpnaccesslevelexceptions) | **GET** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions | Get a list of Message VPN Access-Level Exception objects.
*OauthProfileApi* | [**GetOauthProfileAccessLevelGroups**](docs/OauthProfileApi.md#getoauthprofileaccesslevelgroups) | **GET** /oauthProfiles/{oauthProfileName}/accessLevelGroups | Get a list of Group Access Level objects.
*OauthProfileApi* | [**GetOauthProfileClientAllowedHost**](docs/OauthProfileApi.md#getoauthprofileclientallowedhost) | **GET** /oauthProfiles/{oauthProfileName}/clientAllowedHosts/{allowedHost} | Get an Allowed Host Value object.
*OauthProfileApi* | [**GetOauthProfileClientAllowedHosts**](docs/OauthProfileApi.md#getoauthprofileclientallowedhosts) | **GET** /oauthProfiles/{oauthProfileName}/clientAllowedHosts | Get a list of Allowed Host Value objects.
*OauthProfileApi* | [**GetOauthProfileClientAuthorizationParameter**](docs/OauthProfileApi.md#getoauthprofileclientauthorizationparameter) | **GET** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName} | Get an Authorization Parameter object.
*OauthProfileApi* | [**GetOauthProfileClientAuthorizationParameters**](docs/OauthProfileApi.md#getoauthprofileclientauthorizationparameters) | **GET** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters | Get a list of Authorization Parameter objects.
*OauthProfileApi* | [**GetOauthProfileClientRequiredClaim**](docs/OauthProfileApi.md#getoauthprofileclientrequiredclaim) | **GET** /oauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName} | Get a Required Claim object.
*OauthProfileApi* | [**GetOauthProfileClientRequiredClaims**](docs/OauthProfileApi.md#getoauthprofileclientrequiredclaims) | **GET** /oauthProfiles/{oauthProfileName}/clientRequiredClaims | Get a list of Required Claim objects.
*OauthProfileApi* | [**GetOauthProfileDefaultMsgVpnAccessLevelException**](docs/OauthProfileApi.md#getoauthprofiledefaultmsgvpnaccesslevelexception) | **GET** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName} | Get a Message VPN Access-Level Exception object.
*OauthProfileApi* | [**GetOauthProfileDefaultMsgVpnAccessLevelExceptions**](docs/OauthProfileApi.md#getoauthprofiledefaultmsgvpnaccesslevelexceptions) | **GET** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions | Get a list of Message VPN Access-Level Exception objects.
*OauthProfileApi* | [**GetOauthProfileResourceServerRequiredClaim**](docs/OauthProfileApi.md#getoauthprofileresourceserverrequiredclaim) | **GET** /oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName} | Get a Required Claim object.
*OauthProfileApi* | [**GetOauthProfileResourceServerRequiredClaims**](docs/OauthProfileApi.md#getoauthprofileresourceserverrequiredclaims) | **GET** /oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims | Get a list of Required Claim objects.
*OauthProfileApi* | [**GetOauthProfiles**](docs/OauthProfileApi.md#getoauthprofiles) | **GET** /oauthProfiles | Get a list of OAuth Profile objects.
*OauthProfileApi* | [**ReplaceOauthProfile**](docs/OauthProfileApi.md#replaceoauthprofile) | **PUT** /oauthProfiles/{oauthProfileName} | Replace an OAuth Profile object.
*OauthProfileApi* | [**ReplaceOauthProfileAccessLevelGroup**](docs/OauthProfileApi.md#replaceoauthprofileaccesslevelgroup) | **PUT** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName} | Replace a Group Access Level object.
*OauthProfileApi* | [**ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](docs/OauthProfileApi.md#replaceoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **PUT** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName} | Replace a Message VPN Access-Level Exception object.
*OauthProfileApi* | [**ReplaceOauthProfileClientAuthorizationParameter**](docs/OauthProfileApi.md#replaceoauthprofileclientauthorizationparameter) | **PUT** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName} | Replace an Authorization Parameter object.
*OauthProfileApi* | [**ReplaceOauthProfileDefaultMsgVpnAccessLevelException**](docs/OauthProfileApi.md#replaceoauthprofiledefaultmsgvpnaccesslevelexception) | **PUT** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName} | Replace a Message VPN Access-Level Exception object.
*OauthProfileApi* | [**UpdateOauthProfile**](docs/OauthProfileApi.md#updateoauthprofile) | **PATCH** /oauthProfiles/{oauthProfileName} | Update an OAuth Profile object.
*OauthProfileApi* | [**UpdateOauthProfileAccessLevelGroup**](docs/OauthProfileApi.md#updateoauthprofileaccesslevelgroup) | **PATCH** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName} | Update a Group Access Level object.
*OauthProfileApi* | [**UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException**](docs/OauthProfileApi.md#updateoauthprofileaccesslevelgroupmsgvpnaccesslevelexception) | **PATCH** /oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName} | Update a Message VPN Access-Level Exception object.
*OauthProfileApi* | [**UpdateOauthProfileClientAuthorizationParameter**](docs/OauthProfileApi.md#updateoauthprofileclientauthorizationparameter) | **PATCH** /oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName} | Update an Authorization Parameter object.
*OauthProfileApi* | [**UpdateOauthProfileDefaultMsgVpnAccessLevelException**](docs/OauthProfileApi.md#updateoauthprofiledefaultmsgvpnaccesslevelexception) | **PATCH** /oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName} | Update a Message VPN Access-Level Exception object.
*QueueApi* | [**CreateMsgVpnQueue**](docs/QueueApi.md#createmsgvpnqueue) | **POST** /msgVpns/{msgVpnName}/queues | Create a Queue object.
*QueueApi* | [**CreateMsgVpnQueueSubscription**](docs/QueueApi.md#createmsgvpnqueuesubscription) | **POST** /msgVpns/{msgVpnName}/queues/{queueName}/subscriptions | Create a Queue Subscription object.
*QueueApi* | [**DeleteMsgVpnQueue**](docs/QueueApi.md#deletemsgvpnqueue) | **DELETE** /msgVpns/{msgVpnName}/queues/{queueName} | Delete a Queue object.
*QueueApi* | [**DeleteMsgVpnQueueSubscription**](docs/QueueApi.md#deletemsgvpnqueuesubscription) | **DELETE** /msgVpns/{msgVpnName}/queues/{queueName}/subscriptions/{subscriptionTopic} | Delete a Queue Subscription object.
*QueueApi* | [**GetMsgVpnQueue**](docs/QueueApi.md#getmsgvpnqueue) | **GET** /msgVpns/{msgVpnName}/queues/{queueName} | Get a Queue object.
*QueueApi* | [**GetMsgVpnQueueSubscription**](docs/QueueApi.md#getmsgvpnqueuesubscription) | **GET** /msgVpns/{msgVpnName}/queues/{queueName}/subscriptions/{subscriptionTopic} | Get a Queue Subscription object.
*QueueApi* | [**GetMsgVpnQueueSubscriptions**](docs/QueueApi.md#getmsgvpnqueuesubscriptions) | **GET** /msgVpns/{msgVpnName}/queues/{queueName}/subscriptions | Get a list of Queue Subscription objects.
*QueueApi* | [**GetMsgVpnQueues**](docs/QueueApi.md#getmsgvpnqueues) | **GET** /msgVpns/{msgVpnName}/queues | Get a list of Queue objects.
*QueueApi* | [**ReplaceMsgVpnQueue**](docs/QueueApi.md#replacemsgvpnqueue) | **PUT** /msgVpns/{msgVpnName}/queues/{queueName} | Replace a Queue object.
*QueueApi* | [**UpdateMsgVpnQueue**](docs/QueueApi.md#updatemsgvpnqueue) | **PATCH** /msgVpns/{msgVpnName}/queues/{queueName} | Update a Queue object.
*QueueTemplateApi* | [**CreateMsgVpnQueueTemplate**](docs/QueueTemplateApi.md#createmsgvpnqueuetemplate) | **POST** /msgVpns/{msgVpnName}/queueTemplates | Create a Queue Template object.
*QueueTemplateApi* | [**DeleteMsgVpnQueueTemplate**](docs/QueueTemplateApi.md#deletemsgvpnqueuetemplate) | **DELETE** /msgVpns/{msgVpnName}/queueTemplates/{queueTemplateName} | Delete a Queue Template object.
*QueueTemplateApi* | [**GetMsgVpnQueueTemplate**](docs/QueueTemplateApi.md#getmsgvpnqueuetemplate) | **GET** /msgVpns/{msgVpnName}/queueTemplates/{queueTemplateName} | Get a Queue Template object.
*QueueTemplateApi* | [**GetMsgVpnQueueTemplates**](docs/QueueTemplateApi.md#getmsgvpnqueuetemplates) | **GET** /msgVpns/{msgVpnName}/queueTemplates | Get a list of Queue Template objects.
*QueueTemplateApi* | [**ReplaceMsgVpnQueueTemplate**](docs/QueueTemplateApi.md#replacemsgvpnqueuetemplate) | **PUT** /msgVpns/{msgVpnName}/queueTemplates/{queueTemplateName} | Replace a Queue Template object.
*QueueTemplateApi* | [**UpdateMsgVpnQueueTemplate**](docs/QueueTemplateApi.md#updatemsgvpnqueuetemplate) | **PATCH** /msgVpns/{msgVpnName}/queueTemplates/{queueTemplateName} | Update a Queue Template object.
*ReplayLogApi* | [**CreateMsgVpnReplayLog**](docs/ReplayLogApi.md#createmsgvpnreplaylog) | **POST** /msgVpns/{msgVpnName}/replayLogs | Create a Replay Log object.
*ReplayLogApi* | [**CreateMsgVpnReplayLogTopicFilterSubscription**](docs/ReplayLogApi.md#createmsgvpnreplaylogtopicfiltersubscription) | **POST** /msgVpns/{msgVpnName}/replayLogs/{replayLogName}/topicFilterSubscriptions | Create a Topic Filter Subscription object.
*ReplayLogApi* | [**DeleteMsgVpnReplayLog**](docs/ReplayLogApi.md#deletemsgvpnreplaylog) | **DELETE** /msgVpns/{msgVpnName}/replayLogs/{replayLogName} | Delete a Replay Log object.
*ReplayLogApi* | [**DeleteMsgVpnReplayLogTopicFilterSubscription**](docs/ReplayLogApi.md#deletemsgvpnreplaylogtopicfiltersubscription) | **DELETE** /msgVpns/{msgVpnName}/replayLogs/{replayLogName}/topicFilterSubscriptions/{topicFilterSubscription} | Delete a Topic Filter Subscription object.
*ReplayLogApi* | [**GetMsgVpnReplayLog**](docs/ReplayLogApi.md#getmsgvpnreplaylog) | **GET** /msgVpns/{msgVpnName}/replayLogs/{replayLogName} | Get a Replay Log object.
*ReplayLogApi* | [**GetMsgVpnReplayLogTopicFilterSubscription**](docs/ReplayLogApi.md#getmsgvpnreplaylogtopicfiltersubscription) | **GET** /msgVpns/{msgVpnName}/replayLogs/{replayLogName}/topicFilterSubscriptions/{topicFilterSubscription} | Get a Topic Filter Subscription object.
*ReplayLogApi* | [**GetMsgVpnReplayLogTopicFilterSubscriptions**](docs/ReplayLogApi.md#getmsgvpnreplaylogtopicfiltersubscriptions) | **GET** /msgVpns/{msgVpnName}/replayLogs/{replayLogName}/topicFilterSubscriptions | Get a list of Topic Filter Subscription objects.
*ReplayLogApi* | [**GetMsgVpnReplayLogs**](docs/ReplayLogApi.md#getmsgvpnreplaylogs) | **GET** /msgVpns/{msgVpnName}/replayLogs | Get a list of Replay Log objects.
*ReplayLogApi* | [**ReplaceMsgVpnReplayLog**](docs/ReplayLogApi.md#replacemsgvpnreplaylog) | **PUT** /msgVpns/{msgVpnName}/replayLogs/{replayLogName} | Replace a Replay Log object.
*ReplayLogApi* | [**UpdateMsgVpnReplayLog**](docs/ReplayLogApi.md#updatemsgvpnreplaylog) | **PATCH** /msgVpns/{msgVpnName}/replayLogs/{replayLogName} | Update a Replay Log object.
*ReplicatedTopicApi* | [**CreateMsgVpnReplicatedTopic**](docs/ReplicatedTopicApi.md#createmsgvpnreplicatedtopic) | **POST** /msgVpns/{msgVpnName}/replicatedTopics | Create a Replicated Topic object.
*ReplicatedTopicApi* | [**DeleteMsgVpnReplicatedTopic**](docs/ReplicatedTopicApi.md#deletemsgvpnreplicatedtopic) | **DELETE** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Delete a Replicated Topic object.
*ReplicatedTopicApi* | [**GetMsgVpnReplicatedTopic**](docs/ReplicatedTopicApi.md#getmsgvpnreplicatedtopic) | **GET** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Get a Replicated Topic object.
*ReplicatedTopicApi* | [**GetMsgVpnReplicatedTopics**](docs/ReplicatedTopicApi.md#getmsgvpnreplicatedtopics) | **GET** /msgVpns/{msgVpnName}/replicatedTopics | Get a list of Replicated Topic objects.
*ReplicatedTopicApi* | [**ReplaceMsgVpnReplicatedTopic**](docs/ReplicatedTopicApi.md#replacemsgvpnreplicatedtopic) | **PUT** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Replace a Replicated Topic object.
*ReplicatedTopicApi* | [**UpdateMsgVpnReplicatedTopic**](docs/ReplicatedTopicApi.md#updatemsgvpnreplicatedtopic) | **PATCH** /msgVpns/{msgVpnName}/replicatedTopics/{replicatedTopic} | Update a Replicated Topic object.
*RestDeliveryPointApi* | [**CreateMsgVpnRestDeliveryPoint**](docs/RestDeliveryPointApi.md#createmsgvpnrestdeliverypoint) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints | Create a REST Delivery Point object.
*RestDeliveryPointApi* | [**CreateMsgVpnRestDeliveryPointQueueBinding**](docs/RestDeliveryPointApi.md#createmsgvpnrestdeliverypointqueuebinding) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings | Create a Queue Binding object.
*RestDeliveryPointApi* | [**CreateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/RestDeliveryPointApi.md#createmsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders | Create a Protected Request Header object.
*RestDeliveryPointApi* | [**CreateMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/RestDeliveryPointApi.md#createmsgvpnrestdeliverypointqueuebindingrequestheader) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders | Create a Request Header object.
*RestDeliveryPointApi* | [**CreateMsgVpnRestDeliveryPointRestConsumer**](docs/RestDeliveryPointApi.md#createmsgvpnrestdeliverypointrestconsumer) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers | Create a REST Consumer object.
*RestDeliveryPointApi* | [**CreateMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**](docs/RestDeliveryPointApi.md#createmsgvpnrestdeliverypointrestconsumeroauthjwtclaim) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims | Create a Claim object.
*RestDeliveryPointApi* | [**CreateMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**](docs/RestDeliveryPointApi.md#createmsgvpnrestdeliverypointrestconsumertlstrustedcommonname) | **POST** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames | Create a Trusted Common Name object.
*RestDeliveryPointApi* | [**DeleteMsgVpnRestDeliveryPoint**](docs/RestDeliveryPointApi.md#deletemsgvpnrestdeliverypoint) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Delete a REST Delivery Point object.
*RestDeliveryPointApi* | [**DeleteMsgVpnRestDeliveryPointQueueBinding**](docs/RestDeliveryPointApi.md#deletemsgvpnrestdeliverypointqueuebinding) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Delete a Queue Binding object.
*RestDeliveryPointApi* | [**DeleteMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/RestDeliveryPointApi.md#deletemsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Delete a Protected Request Header object.
*RestDeliveryPointApi* | [**DeleteMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/RestDeliveryPointApi.md#deletemsgvpnrestdeliverypointqueuebindingrequestheader) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Delete a Request Header object.
*RestDeliveryPointApi* | [**DeleteMsgVpnRestDeliveryPointRestConsumer**](docs/RestDeliveryPointApi.md#deletemsgvpnrestdeliverypointrestconsumer) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Delete a REST Consumer object.
*RestDeliveryPointApi* | [**DeleteMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**](docs/RestDeliveryPointApi.md#deletemsgvpnrestdeliverypointrestconsumeroauthjwtclaim) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims/{oauthJwtClaimName} | Delete a Claim object.
*RestDeliveryPointApi* | [**DeleteMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**](docs/RestDeliveryPointApi.md#deletemsgvpnrestdeliverypointrestconsumertlstrustedcommonname) | **DELETE** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Delete a Trusted Common Name object.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPoint**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypoint) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Get a REST Delivery Point object.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPointQueueBinding**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypointqueuebinding) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Get a Queue Binding object.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Get a Protected Request Header object.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaders**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypointqueuebindingprotectedrequestheaders) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders | Get a list of Protected Request Header objects.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypointqueuebindingrequestheader) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Get a Request Header object.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPointQueueBindingRequestHeaders**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypointqueuebindingrequestheaders) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders | Get a list of Request Header objects.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPointQueueBindings**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypointqueuebindings) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings | Get a list of Queue Binding objects.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPointRestConsumer**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypointrestconsumer) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Get a REST Consumer object.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaim**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypointrestconsumeroauthjwtclaim) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims/{oauthJwtClaimName} | Get a Claim object.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPointRestConsumerOauthJwtClaims**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypointrestconsumeroauthjwtclaims) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/oauthJwtClaims | Get a list of Claim objects.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypointrestconsumertlstrustedcommonname) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames/{tlsTrustedCommonName} | Get a Trusted Common Name object.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNames**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypointrestconsumertlstrustedcommonnames) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName}/tlsTrustedCommonNames | Get a list of Trusted Common Name objects.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPointRestConsumers**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypointrestconsumers) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers | Get a list of REST Consumer objects.
*RestDeliveryPointApi* | [**GetMsgVpnRestDeliveryPoints**](docs/RestDeliveryPointApi.md#getmsgvpnrestdeliverypoints) | **GET** /msgVpns/{msgVpnName}/restDeliveryPoints | Get a list of REST Delivery Point objects.
*RestDeliveryPointApi* | [**ReplaceMsgVpnRestDeliveryPoint**](docs/RestDeliveryPointApi.md#replacemsgvpnrestdeliverypoint) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Replace a REST Delivery Point object.
*RestDeliveryPointApi* | [**ReplaceMsgVpnRestDeliveryPointQueueBinding**](docs/RestDeliveryPointApi.md#replacemsgvpnrestdeliverypointqueuebinding) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Replace a Queue Binding object.
*RestDeliveryPointApi* | [**ReplaceMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/RestDeliveryPointApi.md#replacemsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Replace a Protected Request Header object.
*RestDeliveryPointApi* | [**ReplaceMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/RestDeliveryPointApi.md#replacemsgvpnrestdeliverypointqueuebindingrequestheader) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Replace a Request Header object.
*RestDeliveryPointApi* | [**ReplaceMsgVpnRestDeliveryPointRestConsumer**](docs/RestDeliveryPointApi.md#replacemsgvpnrestdeliverypointrestconsumer) | **PUT** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Replace a REST Consumer object.
*RestDeliveryPointApi* | [**UpdateMsgVpnRestDeliveryPoint**](docs/RestDeliveryPointApi.md#updatemsgvpnrestdeliverypoint) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName} | Update a REST Delivery Point object.
*RestDeliveryPointApi* | [**UpdateMsgVpnRestDeliveryPointQueueBinding**](docs/RestDeliveryPointApi.md#updatemsgvpnrestdeliverypointqueuebinding) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName} | Update a Queue Binding object.
*RestDeliveryPointApi* | [**UpdateMsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader**](docs/RestDeliveryPointApi.md#updatemsgvpnrestdeliverypointqueuebindingprotectedrequestheader) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/protectedRequestHeaders/{headerName} | Update a Protected Request Header object.
*RestDeliveryPointApi* | [**UpdateMsgVpnRestDeliveryPointQueueBindingRequestHeader**](docs/RestDeliveryPointApi.md#updatemsgvpnrestdeliverypointqueuebindingrequestheader) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/queueBindings/{queueBindingName}/requestHeaders/{headerName} | Update a Request Header object.
*RestDeliveryPointApi* | [**UpdateMsgVpnRestDeliveryPointRestConsumer**](docs/RestDeliveryPointApi.md#updatemsgvpnrestdeliverypointrestconsumer) | **PATCH** /msgVpns/{msgVpnName}/restDeliveryPoints/{restDeliveryPointName}/restConsumers/{restConsumerName} | Update a REST Consumer object.
*SystemInformationApi* | [**GetSystemInformation**](docs/SystemInformationApi.md#getsysteminformation) | **GET** /systemInformation | Get a System Information object.
*TelemetryProfileApi* | [**CreateMsgVpnTelemetryProfile**](docs/TelemetryProfileApi.md#createmsgvpntelemetryprofile) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles | Create a Telemetry Profile object.
*TelemetryProfileApi* | [**CreateMsgVpnTelemetryProfileReceiverAclConnectException**](docs/TelemetryProfileApi.md#createmsgvpntelemetryprofilereceiveraclconnectexception) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions | Create a Receiver ACL Connect Exception object.
*TelemetryProfileApi* | [**CreateMsgVpnTelemetryProfileTraceFilter**](docs/TelemetryProfileApi.md#createmsgvpntelemetryprofiletracefilter) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters | Create a Trace Filter object.
*TelemetryProfileApi* | [**CreateMsgVpnTelemetryProfileTraceFilterSubscription**](docs/TelemetryProfileApi.md#createmsgvpntelemetryprofiletracefiltersubscription) | **POST** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions | Create a Telemetry Trace Filter Subscription object.
*TelemetryProfileApi* | [**DeleteMsgVpnTelemetryProfile**](docs/TelemetryProfileApi.md#deletemsgvpntelemetryprofile) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Delete a Telemetry Profile object.
*TelemetryProfileApi* | [**DeleteMsgVpnTelemetryProfileReceiverAclConnectException**](docs/TelemetryProfileApi.md#deletemsgvpntelemetryprofilereceiveraclconnectexception) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions/{receiverAclConnectExceptionAddress} | Delete a Receiver ACL Connect Exception object.
*TelemetryProfileApi* | [**DeleteMsgVpnTelemetryProfileTraceFilter**](docs/TelemetryProfileApi.md#deletemsgvpntelemetryprofiletracefilter) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Delete a Trace Filter object.
*TelemetryProfileApi* | [**DeleteMsgVpnTelemetryProfileTraceFilterSubscription**](docs/TelemetryProfileApi.md#deletemsgvpntelemetryprofiletracefiltersubscription) | **DELETE** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions/{subscription},{subscriptionSyntax} | Delete a Telemetry Trace Filter Subscription object.
*TelemetryProfileApi* | [**GetMsgVpnTelemetryProfile**](docs/TelemetryProfileApi.md#getmsgvpntelemetryprofile) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Get a Telemetry Profile object.
*TelemetryProfileApi* | [**GetMsgVpnTelemetryProfileReceiverAclConnectException**](docs/TelemetryProfileApi.md#getmsgvpntelemetryprofilereceiveraclconnectexception) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions/{receiverAclConnectExceptionAddress} | Get a Receiver ACL Connect Exception object.
*TelemetryProfileApi* | [**GetMsgVpnTelemetryProfileReceiverAclConnectExceptions**](docs/TelemetryProfileApi.md#getmsgvpntelemetryprofilereceiveraclconnectexceptions) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions | Get a list of Receiver ACL Connect Exception objects.
*TelemetryProfileApi* | [**GetMsgVpnTelemetryProfileTraceFilter**](docs/TelemetryProfileApi.md#getmsgvpntelemetryprofiletracefilter) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Get a Trace Filter object.
*TelemetryProfileApi* | [**GetMsgVpnTelemetryProfileTraceFilterSubscription**](docs/TelemetryProfileApi.md#getmsgvpntelemetryprofiletracefiltersubscription) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions/{subscription},{subscriptionSyntax} | Get a Telemetry Trace Filter Subscription object.
*TelemetryProfileApi* | [**GetMsgVpnTelemetryProfileTraceFilterSubscriptions**](docs/TelemetryProfileApi.md#getmsgvpntelemetryprofiletracefiltersubscriptions) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions | Get a list of Telemetry Trace Filter Subscription objects.
*TelemetryProfileApi* | [**GetMsgVpnTelemetryProfileTraceFilters**](docs/TelemetryProfileApi.md#getmsgvpntelemetryprofiletracefilters) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters | Get a list of Trace Filter objects.
*TelemetryProfileApi* | [**GetMsgVpnTelemetryProfiles**](docs/TelemetryProfileApi.md#getmsgvpntelemetryprofiles) | **GET** /msgVpns/{msgVpnName}/telemetryProfiles | Get a list of Telemetry Profile objects.
*TelemetryProfileApi* | [**ReplaceMsgVpnTelemetryProfile**](docs/TelemetryProfileApi.md#replacemsgvpntelemetryprofile) | **PUT** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Replace a Telemetry Profile object.
*TelemetryProfileApi* | [**ReplaceMsgVpnTelemetryProfileTraceFilter**](docs/TelemetryProfileApi.md#replacemsgvpntelemetryprofiletracefilter) | **PUT** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Replace a Trace Filter object.
*TelemetryProfileApi* | [**UpdateMsgVpnTelemetryProfile**](docs/TelemetryProfileApi.md#updatemsgvpntelemetryprofile) | **PATCH** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName} | Update a Telemetry Profile object.
*TelemetryProfileApi* | [**UpdateMsgVpnTelemetryProfileTraceFilter**](docs/TelemetryProfileApi.md#updatemsgvpntelemetryprofiletracefilter) | **PATCH** /msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName} | Update a Trace Filter object.
*TopicEndpointApi* | [**CreateMsgVpnTopicEndpoint**](docs/TopicEndpointApi.md#createmsgvpntopicendpoint) | **POST** /msgVpns/{msgVpnName}/topicEndpoints | Create a Topic Endpoint object.
*TopicEndpointApi* | [**DeleteMsgVpnTopicEndpoint**](docs/TopicEndpointApi.md#deletemsgvpntopicendpoint) | **DELETE** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Delete a Topic Endpoint object.
*TopicEndpointApi* | [**GetMsgVpnTopicEndpoint**](docs/TopicEndpointApi.md#getmsgvpntopicendpoint) | **GET** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Get a Topic Endpoint object.
*TopicEndpointApi* | [**GetMsgVpnTopicEndpoints**](docs/TopicEndpointApi.md#getmsgvpntopicendpoints) | **GET** /msgVpns/{msgVpnName}/topicEndpoints | Get a list of Topic Endpoint objects.
*TopicEndpointApi* | [**ReplaceMsgVpnTopicEndpoint**](docs/TopicEndpointApi.md#replacemsgvpntopicendpoint) | **PUT** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Replace a Topic Endpoint object.
*TopicEndpointApi* | [**UpdateMsgVpnTopicEndpoint**](docs/TopicEndpointApi.md#updatemsgvpntopicendpoint) | **PATCH** /msgVpns/{msgVpnName}/topicEndpoints/{topicEndpointName} | Update a Topic Endpoint object.
*TopicEndpointTemplateApi* | [**CreateMsgVpnTopicEndpointTemplate**](docs/TopicEndpointTemplateApi.md#createmsgvpntopicendpointtemplate) | **POST** /msgVpns/{msgVpnName}/topicEndpointTemplates | Create a Topic Endpoint Template object.
*TopicEndpointTemplateApi* | [**DeleteMsgVpnTopicEndpointTemplate**](docs/TopicEndpointTemplateApi.md#deletemsgvpntopicendpointtemplate) | **DELETE** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Delete a Topic Endpoint Template object.
*TopicEndpointTemplateApi* | [**GetMsgVpnTopicEndpointTemplate**](docs/TopicEndpointTemplateApi.md#getmsgvpntopicendpointtemplate) | **GET** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Get a Topic Endpoint Template object.
*TopicEndpointTemplateApi* | [**GetMsgVpnTopicEndpointTemplates**](docs/TopicEndpointTemplateApi.md#getmsgvpntopicendpointtemplates) | **GET** /msgVpns/{msgVpnName}/topicEndpointTemplates | Get a list of Topic Endpoint Template objects.
*TopicEndpointTemplateApi* | [**ReplaceMsgVpnTopicEndpointTemplate**](docs/TopicEndpointTemplateApi.md#replacemsgvpntopicendpointtemplate) | **PUT** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Replace a Topic Endpoint Template object.
*TopicEndpointTemplateApi* | [**UpdateMsgVpnTopicEndpointTemplate**](docs/TopicEndpointTemplateApi.md#updatemsgvpntopicendpointtemplate) | **PATCH** /msgVpns/{msgVpnName}/topicEndpointTemplates/{topicEndpointTemplateName} | Update a Topic Endpoint Template object.
*VirtualHostnameApi* | [**CreateVirtualHostname**](docs/VirtualHostnameApi.md#createvirtualhostname) | **POST** /virtualHostnames | Create a Virtual Hostname object.
*VirtualHostnameApi* | [**DeleteVirtualHostname**](docs/VirtualHostnameApi.md#deletevirtualhostname) | **DELETE** /virtualHostnames/{virtualHostname} | Delete a Virtual Hostname object.
*VirtualHostnameApi* | [**GetVirtualHostname**](docs/VirtualHostnameApi.md#getvirtualhostname) | **GET** /virtualHostnames/{virtualHostname} | Get a Virtual Hostname object.
*VirtualHostnameApi* | [**GetVirtualHostnames**](docs/VirtualHostnameApi.md#getvirtualhostnames) | **GET** /virtualHostnames | Get a list of Virtual Hostname objects.
*VirtualHostnameApi* | [**ReplaceVirtualHostname**](docs/VirtualHostnameApi.md#replacevirtualhostname) | **PUT** /virtualHostnames/{virtualHostname} | Replace a Virtual Hostname object.
*VirtualHostnameApi* | [**UpdateVirtualHostname**](docs/VirtualHostnameApi.md#updatevirtualhostname) | **PATCH** /virtualHostnames/{virtualHostname} | Update a Virtual Hostname object.

<a name="documentation-for-models"></a>
## Documentation for Models

 - [Model.About](docs/About.md)
 - [Model.AboutApi](docs/AboutApi.md)
 - [Model.AboutApiLinks](docs/AboutApiLinks.md)
 - [Model.AboutApiResponse](docs/AboutApiResponse.md)
 - [Model.AboutLinks](docs/AboutLinks.md)
 - [Model.AboutResponse](docs/AboutResponse.md)
 - [Model.AboutUser](docs/AboutUser.md)
 - [Model.AboutUserLinks](docs/AboutUserLinks.md)
 - [Model.AboutUserMsgVpn](docs/AboutUserMsgVpn.md)
 - [Model.AboutUserMsgVpnLinks](docs/AboutUserMsgVpnLinks.md)
 - [Model.AboutUserMsgVpnResponse](docs/AboutUserMsgVpnResponse.md)
 - [Model.AboutUserMsgVpnsResponse](docs/AboutUserMsgVpnsResponse.md)
 - [Model.AboutUserResponse](docs/AboutUserResponse.md)
 - [Model.Broker](docs/Broker.md)
 - [Model.BrokerLinks](docs/BrokerLinks.md)
 - [Model.BrokerResponse](docs/BrokerResponse.md)
 - [Model.CertAuthoritiesResponse](docs/CertAuthoritiesResponse.md)
 - [Model.CertAuthority](docs/CertAuthority.md)
 - [Model.CertAuthorityLinks](docs/CertAuthorityLinks.md)
 - [Model.CertAuthorityOcspTlsTrustedCommonName](docs/CertAuthorityOcspTlsTrustedCommonName.md)
 - [Model.CertAuthorityOcspTlsTrustedCommonNameLinks](docs/CertAuthorityOcspTlsTrustedCommonNameLinks.md)
 - [Model.CertAuthorityOcspTlsTrustedCommonNameResponse](docs/CertAuthorityOcspTlsTrustedCommonNameResponse.md)
 - [Model.CertAuthorityOcspTlsTrustedCommonNamesResponse](docs/CertAuthorityOcspTlsTrustedCommonNamesResponse.md)
 - [Model.CertAuthorityResponse](docs/CertAuthorityResponse.md)
 - [Model.ClientCertAuthoritiesResponse](docs/ClientCertAuthoritiesResponse.md)
 - [Model.ClientCertAuthority](docs/ClientCertAuthority.md)
 - [Model.ClientCertAuthorityLinks](docs/ClientCertAuthorityLinks.md)
 - [Model.ClientCertAuthorityOcspTlsTrustedCommonName](docs/ClientCertAuthorityOcspTlsTrustedCommonName.md)
 - [Model.ClientCertAuthorityOcspTlsTrustedCommonNameLinks](docs/ClientCertAuthorityOcspTlsTrustedCommonNameLinks.md)
 - [Model.ClientCertAuthorityOcspTlsTrustedCommonNameResponse](docs/ClientCertAuthorityOcspTlsTrustedCommonNameResponse.md)
 - [Model.ClientCertAuthorityOcspTlsTrustedCommonNamesResponse](docs/ClientCertAuthorityOcspTlsTrustedCommonNamesResponse.md)
 - [Model.ClientCertAuthorityResponse](docs/ClientCertAuthorityResponse.md)
 - [Model.DmrCluster](docs/DmrCluster.md)
 - [Model.DmrClusterCertMatchingRule](docs/DmrClusterCertMatchingRule.md)
 - [Model.DmrClusterCertMatchingRuleAttributeFilter](docs/DmrClusterCertMatchingRuleAttributeFilter.md)
 - [Model.DmrClusterCertMatchingRuleAttributeFilterLinks](docs/DmrClusterCertMatchingRuleAttributeFilterLinks.md)
 - [Model.DmrClusterCertMatchingRuleAttributeFilterResponse](docs/DmrClusterCertMatchingRuleAttributeFilterResponse.md)
 - [Model.DmrClusterCertMatchingRuleAttributeFiltersResponse](docs/DmrClusterCertMatchingRuleAttributeFiltersResponse.md)
 - [Model.DmrClusterCertMatchingRuleCondition](docs/DmrClusterCertMatchingRuleCondition.md)
 - [Model.DmrClusterCertMatchingRuleConditionLinks](docs/DmrClusterCertMatchingRuleConditionLinks.md)
 - [Model.DmrClusterCertMatchingRuleConditionResponse](docs/DmrClusterCertMatchingRuleConditionResponse.md)
 - [Model.DmrClusterCertMatchingRuleConditionsResponse](docs/DmrClusterCertMatchingRuleConditionsResponse.md)
 - [Model.DmrClusterCertMatchingRuleLinks](docs/DmrClusterCertMatchingRuleLinks.md)
 - [Model.DmrClusterCertMatchingRuleResponse](docs/DmrClusterCertMatchingRuleResponse.md)
 - [Model.DmrClusterCertMatchingRulesResponse](docs/DmrClusterCertMatchingRulesResponse.md)
 - [Model.DmrClusterLink](docs/DmrClusterLink.md)
 - [Model.DmrClusterLinkAttribute](docs/DmrClusterLinkAttribute.md)
 - [Model.DmrClusterLinkAttributeLinks](docs/DmrClusterLinkAttributeLinks.md)
 - [Model.DmrClusterLinkAttributeResponse](docs/DmrClusterLinkAttributeResponse.md)
 - [Model.DmrClusterLinkAttributesResponse](docs/DmrClusterLinkAttributesResponse.md)
 - [Model.DmrClusterLinkLinks](docs/DmrClusterLinkLinks.md)
 - [Model.DmrClusterLinkRemoteAddress](docs/DmrClusterLinkRemoteAddress.md)
 - [Model.DmrClusterLinkRemoteAddressLinks](docs/DmrClusterLinkRemoteAddressLinks.md)
 - [Model.DmrClusterLinkRemoteAddressResponse](docs/DmrClusterLinkRemoteAddressResponse.md)
 - [Model.DmrClusterLinkRemoteAddressesResponse](docs/DmrClusterLinkRemoteAddressesResponse.md)
 - [Model.DmrClusterLinkResponse](docs/DmrClusterLinkResponse.md)
 - [Model.DmrClusterLinkTlsTrustedCommonName](docs/DmrClusterLinkTlsTrustedCommonName.md)
 - [Model.DmrClusterLinkTlsTrustedCommonNameLinks](docs/DmrClusterLinkTlsTrustedCommonNameLinks.md)
 - [Model.DmrClusterLinkTlsTrustedCommonNameResponse](docs/DmrClusterLinkTlsTrustedCommonNameResponse.md)
 - [Model.DmrClusterLinkTlsTrustedCommonNamesResponse](docs/DmrClusterLinkTlsTrustedCommonNamesResponse.md)
 - [Model.DmrClusterLinks](docs/DmrClusterLinks.md)
 - [Model.DmrClusterLinksResponse](docs/DmrClusterLinksResponse.md)
 - [Model.DmrClusterResponse](docs/DmrClusterResponse.md)
 - [Model.DmrClustersResponse](docs/DmrClustersResponse.md)
 - [Model.DomainCertAuthoritiesResponse](docs/DomainCertAuthoritiesResponse.md)
 - [Model.DomainCertAuthority](docs/DomainCertAuthority.md)
 - [Model.DomainCertAuthorityLinks](docs/DomainCertAuthorityLinks.md)
 - [Model.DomainCertAuthorityResponse](docs/DomainCertAuthorityResponse.md)
 - [Model.EventThreshold](docs/EventThreshold.md)
 - [Model.EventThresholdByPercent](docs/EventThresholdByPercent.md)
 - [Model.EventThresholdByValue](docs/EventThresholdByValue.md)
 - [Model.MsgVpn](docs/MsgVpn.md)
 - [Model.MsgVpnAclProfile](docs/MsgVpnAclProfile.md)
 - [Model.MsgVpnAclProfileClientConnectException](docs/MsgVpnAclProfileClientConnectException.md)
 - [Model.MsgVpnAclProfileClientConnectExceptionLinks](docs/MsgVpnAclProfileClientConnectExceptionLinks.md)
 - [Model.MsgVpnAclProfileClientConnectExceptionResponse](docs/MsgVpnAclProfileClientConnectExceptionResponse.md)
 - [Model.MsgVpnAclProfileClientConnectExceptionsResponse](docs/MsgVpnAclProfileClientConnectExceptionsResponse.md)
 - [Model.MsgVpnAclProfileLinks](docs/MsgVpnAclProfileLinks.md)
 - [Model.MsgVpnAclProfilePublishException](docs/MsgVpnAclProfilePublishException.md)
 - [Model.MsgVpnAclProfilePublishExceptionLinks](docs/MsgVpnAclProfilePublishExceptionLinks.md)
 - [Model.MsgVpnAclProfilePublishExceptionResponse](docs/MsgVpnAclProfilePublishExceptionResponse.md)
 - [Model.MsgVpnAclProfilePublishExceptionsResponse](docs/MsgVpnAclProfilePublishExceptionsResponse.md)
 - [Model.MsgVpnAclProfilePublishTopicException](docs/MsgVpnAclProfilePublishTopicException.md)
 - [Model.MsgVpnAclProfilePublishTopicExceptionLinks](docs/MsgVpnAclProfilePublishTopicExceptionLinks.md)
 - [Model.MsgVpnAclProfilePublishTopicExceptionResponse](docs/MsgVpnAclProfilePublishTopicExceptionResponse.md)
 - [Model.MsgVpnAclProfilePublishTopicExceptionsResponse](docs/MsgVpnAclProfilePublishTopicExceptionsResponse.md)
 - [Model.MsgVpnAclProfileResponse](docs/MsgVpnAclProfileResponse.md)
 - [Model.MsgVpnAclProfileSubscribeException](docs/MsgVpnAclProfileSubscribeException.md)
 - [Model.MsgVpnAclProfileSubscribeExceptionLinks](docs/MsgVpnAclProfileSubscribeExceptionLinks.md)
 - [Model.MsgVpnAclProfileSubscribeExceptionResponse](docs/MsgVpnAclProfileSubscribeExceptionResponse.md)
 - [Model.MsgVpnAclProfileSubscribeExceptionsResponse](docs/MsgVpnAclProfileSubscribeExceptionsResponse.md)
 - [Model.MsgVpnAclProfileSubscribeShareNameException](docs/MsgVpnAclProfileSubscribeShareNameException.md)
 - [Model.MsgVpnAclProfileSubscribeShareNameExceptionLinks](docs/MsgVpnAclProfileSubscribeShareNameExceptionLinks.md)
 - [Model.MsgVpnAclProfileSubscribeShareNameExceptionResponse](docs/MsgVpnAclProfileSubscribeShareNameExceptionResponse.md)
 - [Model.MsgVpnAclProfileSubscribeShareNameExceptionsResponse](docs/MsgVpnAclProfileSubscribeShareNameExceptionsResponse.md)
 - [Model.MsgVpnAclProfileSubscribeTopicException](docs/MsgVpnAclProfileSubscribeTopicException.md)
 - [Model.MsgVpnAclProfileSubscribeTopicExceptionLinks](docs/MsgVpnAclProfileSubscribeTopicExceptionLinks.md)
 - [Model.MsgVpnAclProfileSubscribeTopicExceptionResponse](docs/MsgVpnAclProfileSubscribeTopicExceptionResponse.md)
 - [Model.MsgVpnAclProfileSubscribeTopicExceptionsResponse](docs/MsgVpnAclProfileSubscribeTopicExceptionsResponse.md)
 - [Model.MsgVpnAclProfilesResponse](docs/MsgVpnAclProfilesResponse.md)
 - [Model.MsgVpnAuthenticationOauthProfile](docs/MsgVpnAuthenticationOauthProfile.md)
 - [Model.MsgVpnAuthenticationOauthProfileClientRequiredClaim](docs/MsgVpnAuthenticationOauthProfileClientRequiredClaim.md)
 - [Model.MsgVpnAuthenticationOauthProfileClientRequiredClaimLinks](docs/MsgVpnAuthenticationOauthProfileClientRequiredClaimLinks.md)
 - [Model.MsgVpnAuthenticationOauthProfileClientRequiredClaimResponse](docs/MsgVpnAuthenticationOauthProfileClientRequiredClaimResponse.md)
 - [Model.MsgVpnAuthenticationOauthProfileClientRequiredClaimsResponse](docs/MsgVpnAuthenticationOauthProfileClientRequiredClaimsResponse.md)
 - [Model.MsgVpnAuthenticationOauthProfileLinks](docs/MsgVpnAuthenticationOauthProfileLinks.md)
 - [Model.MsgVpnAuthenticationOauthProfileResourceServerRequiredClaim](docs/MsgVpnAuthenticationOauthProfileResourceServerRequiredClaim.md)
 - [Model.MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimLinks](docs/MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimLinks.md)
 - [Model.MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimResponse](docs/MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimResponse.md)
 - [Model.MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimsResponse](docs/MsgVpnAuthenticationOauthProfileResourceServerRequiredClaimsResponse.md)
 - [Model.MsgVpnAuthenticationOauthProfileResponse](docs/MsgVpnAuthenticationOauthProfileResponse.md)
 - [Model.MsgVpnAuthenticationOauthProfilesResponse](docs/MsgVpnAuthenticationOauthProfilesResponse.md)
 - [Model.MsgVpnAuthenticationOauthProvider](docs/MsgVpnAuthenticationOauthProvider.md)
 - [Model.MsgVpnAuthenticationOauthProviderLinks](docs/MsgVpnAuthenticationOauthProviderLinks.md)
 - [Model.MsgVpnAuthenticationOauthProviderResponse](docs/MsgVpnAuthenticationOauthProviderResponse.md)
 - [Model.MsgVpnAuthenticationOauthProvidersResponse](docs/MsgVpnAuthenticationOauthProvidersResponse.md)
 - [Model.MsgVpnAuthorizationGroup](docs/MsgVpnAuthorizationGroup.md)
 - [Model.MsgVpnAuthorizationGroupLinks](docs/MsgVpnAuthorizationGroupLinks.md)
 - [Model.MsgVpnAuthorizationGroupResponse](docs/MsgVpnAuthorizationGroupResponse.md)
 - [Model.MsgVpnAuthorizationGroupsResponse](docs/MsgVpnAuthorizationGroupsResponse.md)
 - [Model.MsgVpnBridge](docs/MsgVpnBridge.md)
 - [Model.MsgVpnBridgeLinks](docs/MsgVpnBridgeLinks.md)
 - [Model.MsgVpnBridgeRemoteMsgVpn](docs/MsgVpnBridgeRemoteMsgVpn.md)
 - [Model.MsgVpnBridgeRemoteMsgVpnLinks](docs/MsgVpnBridgeRemoteMsgVpnLinks.md)
 - [Model.MsgVpnBridgeRemoteMsgVpnResponse](docs/MsgVpnBridgeRemoteMsgVpnResponse.md)
 - [Model.MsgVpnBridgeRemoteMsgVpnsResponse](docs/MsgVpnBridgeRemoteMsgVpnsResponse.md)
 - [Model.MsgVpnBridgeRemoteSubscription](docs/MsgVpnBridgeRemoteSubscription.md)
 - [Model.MsgVpnBridgeRemoteSubscriptionLinks](docs/MsgVpnBridgeRemoteSubscriptionLinks.md)
 - [Model.MsgVpnBridgeRemoteSubscriptionResponse](docs/MsgVpnBridgeRemoteSubscriptionResponse.md)
 - [Model.MsgVpnBridgeRemoteSubscriptionsResponse](docs/MsgVpnBridgeRemoteSubscriptionsResponse.md)
 - [Model.MsgVpnBridgeResponse](docs/MsgVpnBridgeResponse.md)
 - [Model.MsgVpnBridgeTlsTrustedCommonName](docs/MsgVpnBridgeTlsTrustedCommonName.md)
 - [Model.MsgVpnBridgeTlsTrustedCommonNameLinks](docs/MsgVpnBridgeTlsTrustedCommonNameLinks.md)
 - [Model.MsgVpnBridgeTlsTrustedCommonNameResponse](docs/MsgVpnBridgeTlsTrustedCommonNameResponse.md)
 - [Model.MsgVpnBridgeTlsTrustedCommonNamesResponse](docs/MsgVpnBridgeTlsTrustedCommonNamesResponse.md)
 - [Model.MsgVpnBridgesResponse](docs/MsgVpnBridgesResponse.md)
 - [Model.MsgVpnCertMatchingRule](docs/MsgVpnCertMatchingRule.md)
 - [Model.MsgVpnCertMatchingRuleAttributeFilter](docs/MsgVpnCertMatchingRuleAttributeFilter.md)
 - [Model.MsgVpnCertMatchingRuleAttributeFilterLinks](docs/MsgVpnCertMatchingRuleAttributeFilterLinks.md)
 - [Model.MsgVpnCertMatchingRuleAttributeFilterResponse](docs/MsgVpnCertMatchingRuleAttributeFilterResponse.md)
 - [Model.MsgVpnCertMatchingRuleAttributeFiltersResponse](docs/MsgVpnCertMatchingRuleAttributeFiltersResponse.md)
 - [Model.MsgVpnCertMatchingRuleCondition](docs/MsgVpnCertMatchingRuleCondition.md)
 - [Model.MsgVpnCertMatchingRuleConditionLinks](docs/MsgVpnCertMatchingRuleConditionLinks.md)
 - [Model.MsgVpnCertMatchingRuleConditionResponse](docs/MsgVpnCertMatchingRuleConditionResponse.md)
 - [Model.MsgVpnCertMatchingRuleConditionsResponse](docs/MsgVpnCertMatchingRuleConditionsResponse.md)
 - [Model.MsgVpnCertMatchingRuleLinks](docs/MsgVpnCertMatchingRuleLinks.md)
 - [Model.MsgVpnCertMatchingRuleResponse](docs/MsgVpnCertMatchingRuleResponse.md)
 - [Model.MsgVpnCertMatchingRulesResponse](docs/MsgVpnCertMatchingRulesResponse.md)
 - [Model.MsgVpnClientProfile](docs/MsgVpnClientProfile.md)
 - [Model.MsgVpnClientProfileLinks](docs/MsgVpnClientProfileLinks.md)
 - [Model.MsgVpnClientProfileResponse](docs/MsgVpnClientProfileResponse.md)
 - [Model.MsgVpnClientProfilesResponse](docs/MsgVpnClientProfilesResponse.md)
 - [Model.MsgVpnClientUsername](docs/MsgVpnClientUsername.md)
 - [Model.MsgVpnClientUsernameAttribute](docs/MsgVpnClientUsernameAttribute.md)
 - [Model.MsgVpnClientUsernameAttributeLinks](docs/MsgVpnClientUsernameAttributeLinks.md)
 - [Model.MsgVpnClientUsernameAttributeResponse](docs/MsgVpnClientUsernameAttributeResponse.md)
 - [Model.MsgVpnClientUsernameAttributesResponse](docs/MsgVpnClientUsernameAttributesResponse.md)
 - [Model.MsgVpnClientUsernameLinks](docs/MsgVpnClientUsernameLinks.md)
 - [Model.MsgVpnClientUsernameResponse](docs/MsgVpnClientUsernameResponse.md)
 - [Model.MsgVpnClientUsernamesResponse](docs/MsgVpnClientUsernamesResponse.md)
 - [Model.MsgVpnDistributedCache](docs/MsgVpnDistributedCache.md)
 - [Model.MsgVpnDistributedCacheCluster](docs/MsgVpnDistributedCacheCluster.md)
 - [Model.MsgVpnDistributedCacheClusterGlobalCachingHomeCluster](docs/MsgVpnDistributedCacheClusterGlobalCachingHomeCluster.md)
 - [Model.MsgVpnDistributedCacheClusterGlobalCachingHomeClusterLinks](docs/MsgVpnDistributedCacheClusterGlobalCachingHomeClusterLinks.md)
 - [Model.MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse](docs/MsgVpnDistributedCacheClusterGlobalCachingHomeClusterResponse.md)
 - [Model.MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix](docs/MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefix.md)
 - [Model.MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixLinks](docs/MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixLinks.md)
 - [Model.MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse](docs/MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixResponse.md)
 - [Model.MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse](docs/MsgVpnDistributedCacheClusterGlobalCachingHomeClusterTopicPrefixesResponse.md)
 - [Model.MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse](docs/MsgVpnDistributedCacheClusterGlobalCachingHomeClustersResponse.md)
 - [Model.MsgVpnDistributedCacheClusterInstance](docs/MsgVpnDistributedCacheClusterInstance.md)
 - [Model.MsgVpnDistributedCacheClusterInstanceLinks](docs/MsgVpnDistributedCacheClusterInstanceLinks.md)
 - [Model.MsgVpnDistributedCacheClusterInstanceResponse](docs/MsgVpnDistributedCacheClusterInstanceResponse.md)
 - [Model.MsgVpnDistributedCacheClusterInstancesResponse](docs/MsgVpnDistributedCacheClusterInstancesResponse.md)
 - [Model.MsgVpnDistributedCacheClusterLinks](docs/MsgVpnDistributedCacheClusterLinks.md)
 - [Model.MsgVpnDistributedCacheClusterResponse](docs/MsgVpnDistributedCacheClusterResponse.md)
 - [Model.MsgVpnDistributedCacheClusterTopic](docs/MsgVpnDistributedCacheClusterTopic.md)
 - [Model.MsgVpnDistributedCacheClusterTopicLinks](docs/MsgVpnDistributedCacheClusterTopicLinks.md)
 - [Model.MsgVpnDistributedCacheClusterTopicResponse](docs/MsgVpnDistributedCacheClusterTopicResponse.md)
 - [Model.MsgVpnDistributedCacheClusterTopicsResponse](docs/MsgVpnDistributedCacheClusterTopicsResponse.md)
 - [Model.MsgVpnDistributedCacheClustersResponse](docs/MsgVpnDistributedCacheClustersResponse.md)
 - [Model.MsgVpnDistributedCacheLinks](docs/MsgVpnDistributedCacheLinks.md)
 - [Model.MsgVpnDistributedCacheResponse](docs/MsgVpnDistributedCacheResponse.md)
 - [Model.MsgVpnDistributedCachesResponse](docs/MsgVpnDistributedCachesResponse.md)
 - [Model.MsgVpnDmrBridge](docs/MsgVpnDmrBridge.md)
 - [Model.MsgVpnDmrBridgeLinks](docs/MsgVpnDmrBridgeLinks.md)
 - [Model.MsgVpnDmrBridgeResponse](docs/MsgVpnDmrBridgeResponse.md)
 - [Model.MsgVpnDmrBridgesResponse](docs/MsgVpnDmrBridgesResponse.md)
 - [Model.MsgVpnJndiConnectionFactoriesResponse](docs/MsgVpnJndiConnectionFactoriesResponse.md)
 - [Model.MsgVpnJndiConnectionFactory](docs/MsgVpnJndiConnectionFactory.md)
 - [Model.MsgVpnJndiConnectionFactoryLinks](docs/MsgVpnJndiConnectionFactoryLinks.md)
 - [Model.MsgVpnJndiConnectionFactoryResponse](docs/MsgVpnJndiConnectionFactoryResponse.md)
 - [Model.MsgVpnJndiQueue](docs/MsgVpnJndiQueue.md)
 - [Model.MsgVpnJndiQueueLinks](docs/MsgVpnJndiQueueLinks.md)
 - [Model.MsgVpnJndiQueueResponse](docs/MsgVpnJndiQueueResponse.md)
 - [Model.MsgVpnJndiQueuesResponse](docs/MsgVpnJndiQueuesResponse.md)
 - [Model.MsgVpnJndiTopic](docs/MsgVpnJndiTopic.md)
 - [Model.MsgVpnJndiTopicLinks](docs/MsgVpnJndiTopicLinks.md)
 - [Model.MsgVpnJndiTopicResponse](docs/MsgVpnJndiTopicResponse.md)
 - [Model.MsgVpnJndiTopicsResponse](docs/MsgVpnJndiTopicsResponse.md)
 - [Model.MsgVpnLinks](docs/MsgVpnLinks.md)
 - [Model.MsgVpnMqttRetainCache](docs/MsgVpnMqttRetainCache.md)
 - [Model.MsgVpnMqttRetainCacheLinks](docs/MsgVpnMqttRetainCacheLinks.md)
 - [Model.MsgVpnMqttRetainCacheResponse](docs/MsgVpnMqttRetainCacheResponse.md)
 - [Model.MsgVpnMqttRetainCachesResponse](docs/MsgVpnMqttRetainCachesResponse.md)
 - [Model.MsgVpnMqttSession](docs/MsgVpnMqttSession.md)
 - [Model.MsgVpnMqttSessionLinks](docs/MsgVpnMqttSessionLinks.md)
 - [Model.MsgVpnMqttSessionResponse](docs/MsgVpnMqttSessionResponse.md)
 - [Model.MsgVpnMqttSessionSubscription](docs/MsgVpnMqttSessionSubscription.md)
 - [Model.MsgVpnMqttSessionSubscriptionLinks](docs/MsgVpnMqttSessionSubscriptionLinks.md)
 - [Model.MsgVpnMqttSessionSubscriptionResponse](docs/MsgVpnMqttSessionSubscriptionResponse.md)
 - [Model.MsgVpnMqttSessionSubscriptionsResponse](docs/MsgVpnMqttSessionSubscriptionsResponse.md)
 - [Model.MsgVpnMqttSessionsResponse](docs/MsgVpnMqttSessionsResponse.md)
 - [Model.MsgVpnQueue](docs/MsgVpnQueue.md)
 - [Model.MsgVpnQueueLinks](docs/MsgVpnQueueLinks.md)
 - [Model.MsgVpnQueueResponse](docs/MsgVpnQueueResponse.md)
 - [Model.MsgVpnQueueSubscription](docs/MsgVpnQueueSubscription.md)
 - [Model.MsgVpnQueueSubscriptionLinks](docs/MsgVpnQueueSubscriptionLinks.md)
 - [Model.MsgVpnQueueSubscriptionResponse](docs/MsgVpnQueueSubscriptionResponse.md)
 - [Model.MsgVpnQueueSubscriptionsResponse](docs/MsgVpnQueueSubscriptionsResponse.md)
 - [Model.MsgVpnQueueTemplate](docs/MsgVpnQueueTemplate.md)
 - [Model.MsgVpnQueueTemplateLinks](docs/MsgVpnQueueTemplateLinks.md)
 - [Model.MsgVpnQueueTemplateResponse](docs/MsgVpnQueueTemplateResponse.md)
 - [Model.MsgVpnQueueTemplatesResponse](docs/MsgVpnQueueTemplatesResponse.md)
 - [Model.MsgVpnQueuesResponse](docs/MsgVpnQueuesResponse.md)
 - [Model.MsgVpnReplayLog](docs/MsgVpnReplayLog.md)
 - [Model.MsgVpnReplayLogLinks](docs/MsgVpnReplayLogLinks.md)
 - [Model.MsgVpnReplayLogResponse](docs/MsgVpnReplayLogResponse.md)
 - [Model.MsgVpnReplayLogTopicFilterSubscription](docs/MsgVpnReplayLogTopicFilterSubscription.md)
 - [Model.MsgVpnReplayLogTopicFilterSubscriptionLinks](docs/MsgVpnReplayLogTopicFilterSubscriptionLinks.md)
 - [Model.MsgVpnReplayLogTopicFilterSubscriptionResponse](docs/MsgVpnReplayLogTopicFilterSubscriptionResponse.md)
 - [Model.MsgVpnReplayLogTopicFilterSubscriptionsResponse](docs/MsgVpnReplayLogTopicFilterSubscriptionsResponse.md)
 - [Model.MsgVpnReplayLogsResponse](docs/MsgVpnReplayLogsResponse.md)
 - [Model.MsgVpnReplicatedTopic](docs/MsgVpnReplicatedTopic.md)
 - [Model.MsgVpnReplicatedTopicLinks](docs/MsgVpnReplicatedTopicLinks.md)
 - [Model.MsgVpnReplicatedTopicResponse](docs/MsgVpnReplicatedTopicResponse.md)
 - [Model.MsgVpnReplicatedTopicsResponse](docs/MsgVpnReplicatedTopicsResponse.md)
 - [Model.MsgVpnResponse](docs/MsgVpnResponse.md)
 - [Model.MsgVpnRestDeliveryPoint](docs/MsgVpnRestDeliveryPoint.md)
 - [Model.MsgVpnRestDeliveryPointLinks](docs/MsgVpnRestDeliveryPointLinks.md)
 - [Model.MsgVpnRestDeliveryPointQueueBinding](docs/MsgVpnRestDeliveryPointQueueBinding.md)
 - [Model.MsgVpnRestDeliveryPointQueueBindingLinks](docs/MsgVpnRestDeliveryPointQueueBindingLinks.md)
 - [Model.MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader](docs/MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeader.md)
 - [Model.MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderLinks](docs/MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderLinks.md)
 - [Model.MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse](docs/MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeaderResponse.md)
 - [Model.MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeadersResponse](docs/MsgVpnRestDeliveryPointQueueBindingProtectedRequestHeadersResponse.md)
 - [Model.MsgVpnRestDeliveryPointQueueBindingRequestHeader](docs/MsgVpnRestDeliveryPointQueueBindingRequestHeader.md)
 - [Model.MsgVpnRestDeliveryPointQueueBindingRequestHeaderLinks](docs/MsgVpnRestDeliveryPointQueueBindingRequestHeaderLinks.md)
 - [Model.MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse](docs/MsgVpnRestDeliveryPointQueueBindingRequestHeaderResponse.md)
 - [Model.MsgVpnRestDeliveryPointQueueBindingRequestHeadersResponse](docs/MsgVpnRestDeliveryPointQueueBindingRequestHeadersResponse.md)
 - [Model.MsgVpnRestDeliveryPointQueueBindingResponse](docs/MsgVpnRestDeliveryPointQueueBindingResponse.md)
 - [Model.MsgVpnRestDeliveryPointQueueBindingsResponse](docs/MsgVpnRestDeliveryPointQueueBindingsResponse.md)
 - [Model.MsgVpnRestDeliveryPointResponse](docs/MsgVpnRestDeliveryPointResponse.md)
 - [Model.MsgVpnRestDeliveryPointRestConsumer](docs/MsgVpnRestDeliveryPointRestConsumer.md)
 - [Model.MsgVpnRestDeliveryPointRestConsumerLinks](docs/MsgVpnRestDeliveryPointRestConsumerLinks.md)
 - [Model.MsgVpnRestDeliveryPointRestConsumerOauthJwtClaim](docs/MsgVpnRestDeliveryPointRestConsumerOauthJwtClaim.md)
 - [Model.MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimLinks](docs/MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimLinks.md)
 - [Model.MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimResponse](docs/MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimResponse.md)
 - [Model.MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimsResponse](docs/MsgVpnRestDeliveryPointRestConsumerOauthJwtClaimsResponse.md)
 - [Model.MsgVpnRestDeliveryPointRestConsumerResponse](docs/MsgVpnRestDeliveryPointRestConsumerResponse.md)
 - [Model.MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName](docs/MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonName.md)
 - [Model.MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameLinks](docs/MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameLinks.md)
 - [Model.MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameResponse](docs/MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNameResponse.md)
 - [Model.MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNamesResponse](docs/MsgVpnRestDeliveryPointRestConsumerTlsTrustedCommonNamesResponse.md)
 - [Model.MsgVpnRestDeliveryPointRestConsumersResponse](docs/MsgVpnRestDeliveryPointRestConsumersResponse.md)
 - [Model.MsgVpnRestDeliveryPointsResponse](docs/MsgVpnRestDeliveryPointsResponse.md)
 - [Model.MsgVpnSequencedTopic](docs/MsgVpnSequencedTopic.md)
 - [Model.MsgVpnSequencedTopicLinks](docs/MsgVpnSequencedTopicLinks.md)
 - [Model.MsgVpnSequencedTopicResponse](docs/MsgVpnSequencedTopicResponse.md)
 - [Model.MsgVpnSequencedTopicsResponse](docs/MsgVpnSequencedTopicsResponse.md)
 - [Model.MsgVpnTelemetryProfile](docs/MsgVpnTelemetryProfile.md)
 - [Model.MsgVpnTelemetryProfileLinks](docs/MsgVpnTelemetryProfileLinks.md)
 - [Model.MsgVpnTelemetryProfileReceiverAclConnectException](docs/MsgVpnTelemetryProfileReceiverAclConnectException.md)
 - [Model.MsgVpnTelemetryProfileReceiverAclConnectExceptionLinks](docs/MsgVpnTelemetryProfileReceiverAclConnectExceptionLinks.md)
 - [Model.MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse](docs/MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse.md)
 - [Model.MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse](docs/MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse.md)
 - [Model.MsgVpnTelemetryProfileResponse](docs/MsgVpnTelemetryProfileResponse.md)
 - [Model.MsgVpnTelemetryProfileTraceFilter](docs/MsgVpnTelemetryProfileTraceFilter.md)
 - [Model.MsgVpnTelemetryProfileTraceFilterLinks](docs/MsgVpnTelemetryProfileTraceFilterLinks.md)
 - [Model.MsgVpnTelemetryProfileTraceFilterResponse](docs/MsgVpnTelemetryProfileTraceFilterResponse.md)
 - [Model.MsgVpnTelemetryProfileTraceFilterSubscription](docs/MsgVpnTelemetryProfileTraceFilterSubscription.md)
 - [Model.MsgVpnTelemetryProfileTraceFilterSubscriptionLinks](docs/MsgVpnTelemetryProfileTraceFilterSubscriptionLinks.md)
 - [Model.MsgVpnTelemetryProfileTraceFilterSubscriptionResponse](docs/MsgVpnTelemetryProfileTraceFilterSubscriptionResponse.md)
 - [Model.MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse](docs/MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse.md)
 - [Model.MsgVpnTelemetryProfileTraceFiltersResponse](docs/MsgVpnTelemetryProfileTraceFiltersResponse.md)
 - [Model.MsgVpnTelemetryProfilesResponse](docs/MsgVpnTelemetryProfilesResponse.md)
 - [Model.MsgVpnTopicEndpoint](docs/MsgVpnTopicEndpoint.md)
 - [Model.MsgVpnTopicEndpointLinks](docs/MsgVpnTopicEndpointLinks.md)
 - [Model.MsgVpnTopicEndpointResponse](docs/MsgVpnTopicEndpointResponse.md)
 - [Model.MsgVpnTopicEndpointTemplate](docs/MsgVpnTopicEndpointTemplate.md)
 - [Model.MsgVpnTopicEndpointTemplateLinks](docs/MsgVpnTopicEndpointTemplateLinks.md)
 - [Model.MsgVpnTopicEndpointTemplateResponse](docs/MsgVpnTopicEndpointTemplateResponse.md)
 - [Model.MsgVpnTopicEndpointTemplatesResponse](docs/MsgVpnTopicEndpointTemplatesResponse.md)
 - [Model.MsgVpnTopicEndpointsResponse](docs/MsgVpnTopicEndpointsResponse.md)
 - [Model.MsgVpnsResponse](docs/MsgVpnsResponse.md)
 - [Model.OauthProfile](docs/OauthProfile.md)
 - [Model.OauthProfileAccessLevelGroup](docs/OauthProfileAccessLevelGroup.md)
 - [Model.OauthProfileAccessLevelGroupLinks](docs/OauthProfileAccessLevelGroupLinks.md)
 - [Model.OauthProfileAccessLevelGroupMsgVpnAccessLevelException](docs/OauthProfileAccessLevelGroupMsgVpnAccessLevelException.md)
 - [Model.OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionLinks](docs/OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionLinks.md)
 - [Model.OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse](docs/OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse.md)
 - [Model.OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse](docs/OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse.md)
 - [Model.OauthProfileAccessLevelGroupResponse](docs/OauthProfileAccessLevelGroupResponse.md)
 - [Model.OauthProfileAccessLevelGroupsResponse](docs/OauthProfileAccessLevelGroupsResponse.md)
 - [Model.OauthProfileClientAllowedHost](docs/OauthProfileClientAllowedHost.md)
 - [Model.OauthProfileClientAllowedHostLinks](docs/OauthProfileClientAllowedHostLinks.md)
 - [Model.OauthProfileClientAllowedHostResponse](docs/OauthProfileClientAllowedHostResponse.md)
 - [Model.OauthProfileClientAllowedHostsResponse](docs/OauthProfileClientAllowedHostsResponse.md)
 - [Model.OauthProfileClientAuthorizationParameter](docs/OauthProfileClientAuthorizationParameter.md)
 - [Model.OauthProfileClientAuthorizationParameterLinks](docs/OauthProfileClientAuthorizationParameterLinks.md)
 - [Model.OauthProfileClientAuthorizationParameterResponse](docs/OauthProfileClientAuthorizationParameterResponse.md)
 - [Model.OauthProfileClientAuthorizationParametersResponse](docs/OauthProfileClientAuthorizationParametersResponse.md)
 - [Model.OauthProfileClientRequiredClaim](docs/OauthProfileClientRequiredClaim.md)
 - [Model.OauthProfileClientRequiredClaimLinks](docs/OauthProfileClientRequiredClaimLinks.md)
 - [Model.OauthProfileClientRequiredClaimResponse](docs/OauthProfileClientRequiredClaimResponse.md)
 - [Model.OauthProfileClientRequiredClaimsResponse](docs/OauthProfileClientRequiredClaimsResponse.md)
 - [Model.OauthProfileDefaultMsgVpnAccessLevelException](docs/OauthProfileDefaultMsgVpnAccessLevelException.md)
 - [Model.OauthProfileDefaultMsgVpnAccessLevelExceptionLinks](docs/OauthProfileDefaultMsgVpnAccessLevelExceptionLinks.md)
 - [Model.OauthProfileDefaultMsgVpnAccessLevelExceptionResponse](docs/OauthProfileDefaultMsgVpnAccessLevelExceptionResponse.md)
 - [Model.OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse](docs/OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse.md)
 - [Model.OauthProfileLinks](docs/OauthProfileLinks.md)
 - [Model.OauthProfileResourceServerRequiredClaim](docs/OauthProfileResourceServerRequiredClaim.md)
 - [Model.OauthProfileResourceServerRequiredClaimLinks](docs/OauthProfileResourceServerRequiredClaimLinks.md)
 - [Model.OauthProfileResourceServerRequiredClaimResponse](docs/OauthProfileResourceServerRequiredClaimResponse.md)
 - [Model.OauthProfileResourceServerRequiredClaimsResponse](docs/OauthProfileResourceServerRequiredClaimsResponse.md)
 - [Model.OauthProfileResponse](docs/OauthProfileResponse.md)
 - [Model.OauthProfilesResponse](docs/OauthProfilesResponse.md)
 - [Model.SempError](docs/SempError.md)
 - [Model.SempMeta](docs/SempMeta.md)
 - [Model.SempMetaOnlyResponse](docs/SempMetaOnlyResponse.md)
 - [Model.SempPaging](docs/SempPaging.md)
 - [Model.SempRequest](docs/SempRequest.md)
 - [Model.SystemInformation](docs/SystemInformation.md)
 - [Model.SystemInformationLinks](docs/SystemInformationLinks.md)
 - [Model.SystemInformationResponse](docs/SystemInformationResponse.md)
 - [Model.VirtualHostname](docs/VirtualHostname.md)
 - [Model.VirtualHostnameLinks](docs/VirtualHostnameLinks.md)
 - [Model.VirtualHostnameResponse](docs/VirtualHostnameResponse.md)
 - [Model.VirtualHostnamesResponse](docs/VirtualHostnamesResponse.md)

<a name="documentation-for-authorization"></a>
## Documentation for Authorization

<a name="basicAuth"></a>
### basicAuth

- **Type**: HTTP basic authentication

