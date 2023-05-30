/* 
 * SEMP (Solace Element Management Protocol)
 *
 * SEMP (starting in `v2`, see note 1) is a RESTful API for configuring, monitoring, and administering a Solace PubSub+ broker.  SEMP uses URIs to address manageable **resources** of the Solace PubSub+ broker. Resources are individual **objects**, **collections** of objects, or (exclusively in the action API) **actions**. This document applies to the following API:   API|Base Path|Purpose|Comments :- --|:- --|:- --|:- -- Configuration|/SEMP/v2/config|Reading and writing config state|See note 2    The following APIs are also available:   API|Base Path|Purpose|Comments :- --|:- --|:- --|:- -- Action|/SEMP/v2/action|Performing actions|See note 2 Monitoring|/SEMP/v2/monitor|Querying operational parameters|See note 2    Resources are always nouns, with individual objects being singular and collections being plural.  Objects within a collection are identified by an `obj-id`, which follows the collection name with the form `collection-name/obj-id`.  Actions within an object are identified by an `action-id`, which follows the object name with the form `obj-id/action-id`.  Some examples:  ``` /SEMP/v2/config/msgVpns                        ; MsgVpn collection /SEMP/v2/config/msgVpns/a                      ; MsgVpn object named \"a\" /SEMP/v2/config/msgVpns/a/queues               ; Queue collection in MsgVpn \"a\" /SEMP/v2/config/msgVpns/a/queues/b             ; Queue object named \"b\" in MsgVpn \"a\" /SEMP/v2/action/msgVpns/a/queues/b/startReplay ; Action that starts a replay on Queue \"b\" in MsgVpn \"a\" /SEMP/v2/monitor/msgVpns/a/clients             ; Client collection in MsgVpn \"a\" /SEMP/v2/monitor/msgVpns/a/clients/c           ; Client object named \"c\" in MsgVpn \"a\" ```  ## Collection Resources  Collections are unordered lists of objects (unless described as otherwise), and are described by JSON arrays. Each item in the array represents an object in the same manner as the individual object would normally be represented. In the configuration API, the creation of a new object is done through its collection resource.  ## Object and Action Resources  Objects are composed of attributes, actions, collections, and other objects. They are described by JSON objects as name/value pairs. The collections and actions of an object are not contained directly in the object's JSON content; rather the content includes an attribute containing a URI which points to the collections and actions. These contained resources must be managed through this URI. At a minimum, every object has one or more identifying attributes, and its own `uri` attribute which contains the URI pointing to itself.  Actions are also composed of attributes, and are described by JSON objects as name/value pairs. Unlike objects, however, they are not members of a collection and cannot be retrieved, only performed. Actions only exist in the action API.  Attributes in an object or action may have any combination of the following properties:   Property|Meaning|Comments :- --|:- --|:- -- Identifying|Attribute is involved in unique identification of the object, and appears in its URI| Const|Attribute value can only be chosen during object creation| Required|Attribute must be provided in the request| Read-Only|Attribute can only be read, not written.|See note 3 Write-Only|Attribute can only be written, not read, unless the attribute is also opaque|See the documentation for the opaque property Requires-Disable|Attribute cannot be changed while the object (or the relevant part of the object) is administratively enabled| Auto-Disable|Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as one or more attributes will be temporarily disabled to apply the change| Deprecated|Attribute is deprecated, and will disappear in the next SEMP version| Opaque|Attribute can be set or retrieved in opaque form when the `opaquePassword` query parameter is present|See the `opaquePassword` query parameter documentation    In some requests, certain attributes may only be provided in certain combinations with other attributes:   Relationship|Meaning :- --|:- -- Requires|Attribute may only be changed by a request if a particular attribute or combination of attributes is also provided in the request Conflicts|Attribute may only be provided in a request if a particular attribute or combination of attributes is not also provided in the request    In the monitoring API, any non-identifying attribute may not be returned in a GET.  ## HTTP Methods  The following HTTP methods manipulate resources in accordance with these general principles. Note that some methods are only used in certain APIs:   Method|Resource|Meaning|Request Body|Response Body|Notes :- --|:- --|:- --|:- --|:- --|:- -- POST|Collection|Create object|Initial attribute values|Object attributes and metadata|Absent attributes are set to default. If object already exists, a 400 error is returned PUT|Object|Update object|New attribute values|Object attributes and metadata|If does not exist, the object is first created. Absent attributes are set to default, with certain exceptions (see note 4) PUT|Action|Performs action|Action arguments|Action metadata| PATCH|Object|Update object|New attribute values|Object attributes and metadata|Absent attributes are left unchanged. If the object does not exist, a 404 error is returned DELETE|Object|Delete object|Empty|Object metadata|If the object does not exist, a 404 is returned GET|Object|Get object|Empty|Object attributes and metadata|If the object does not exist, a 404 is returned GET|Collection|Get collection|Empty|Object attributes and collection metadata|If the collection is empty, then an empty collection is returned with a 200 code    ## Common Query Parameters  The following are some common query parameters that are supported by many method/URI combinations. Individual URIs may document additional parameters. Note that multiple query parameters can be used together in a single URI, separated by the ampersand character. For example:  ``` ; Request for the MsgVpns collection using two hypothetical query parameters ; \"q1\" and \"q2\" with values \"val1\" and \"val2\" respectively /SEMP/v2/config/msgVpns?q1=val1&q2=val2 ```  ### select  Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. Use this query parameter to limit the size of the returned data for each returned object, return only those fields that are desired, or exclude fields that are not desired.  The value of `select` is a comma-separated list of attribute names. If the list contains attribute names that are not prefaced by `-`, only those attributes are included in the response. If the list contains attribute names that are prefaced by `-`, those attributes are excluded from the response. If the list contains both types, then the difference of the first set of attributes and the second set of attributes is returned. If the list is empty (i.e. `select=`), it is treated the same as if no `select` was provided: all attribute are returned.  All attributes that are prefaced by `-` must follow all attributes that are not prefaced by `-`. In addition, each attribute name in the list must match at least one attribute in the object.  Names may include the `*` wildcard (zero or more characters). Nested attribute names are supported using periods (e.g. `parentName.childName`).  Some examples:  ``` ; List of all MsgVpn names /SEMP/v2/config/msgVpns?select=msgVpnName ; List of all MsgVpn and their attributes except for their names /SEMP/v2/config/msgVpns?select=-msgVpnName ; Authentication attributes of MsgVpn \"finance\" /SEMP/v2/config/msgVpns/finance?select=authentication%2A ; All attributes of MsgVpn \"finance\" except for authentication attributes /SEMP/v2/config/msgVpns/finance?select=-authentication%2A ; Access related attributes of Queue \"orderQ\" of MsgVpn \"finance\" /SEMP/v2/config/msgVpns/finance/queues/orderQ?select=owner,permission ```  ### where  Include in the response only objects where certain conditions are true. Use this query parameter to limit which objects are returned to those whose attribute values meet the given conditions.  The value of `where` is a comma-separated list of expressions. All expressions must be true for the object to be included in the response. Each expression takes the form:  ``` expression  = attribute-name OP value OP          = '==' | '!=' | '<' | '>' | '<=' | '>=' ```  `value` may be a number, string, `true`, or `false`, as appropriate for the type of `attribute-name`. Greater-than and less-than comparisons only work for numbers. A `*` in a string `value` is interpreted as a wildcard (zero or more characters). Some examples:  ``` ; Only enabled MsgVpns /SEMP/v2/config/msgVpns?where=enabled%3D%3Dtrue ; Only MsgVpns using basic non-LDAP authentication /SEMP/v2/config/msgVpns?where=authenticationBasicEnabled%3D%3Dtrue,authenticationBasicType%21%3Dldap ; Only MsgVpns that allow more than 100 client connections /SEMP/v2/config/msgVpns?where=maxConnectionCount%3E100 ; Only MsgVpns with msgVpnName starting with \"B\": /SEMP/v2/config/msgVpns?where=msgVpnName%3D%3DB%2A ```  ### count  Limit the count of objects in the response. This can be useful to limit the size of the response for large collections. The minimum value for `count` is `1` and the default is `10`. There is also a per-collection maximum value to limit request handling time.  `count` does not guarantee that a minimum number of objects will be returned. A page may contain fewer than `count` objects or even be empty. Additional objects may nonetheless be available for retrieval on subsequent pages. See the `cursor` query parameter documentation for more information on paging.  For example: ``` ; Up to 25 MsgVpns /SEMP/v2/config/msgVpns?count=25 ```  ### cursor  The cursor, or position, for the next page of objects. Cursors are opaque data that should not be created or interpreted by SEMP clients, and should only be used as described below.  When a request is made for a collection and there may be additional objects available for retrieval that are not included in the initial response, the response will include a `cursorQuery` field containing a cursor. The value of this field can be specified in the `cursor` query parameter of a subsequent request to retrieve the next page of objects.  Applications must continue to use the `cursorQuery` if one is provided in order to retrieve the full set of objects associated with the request, even if a page contains fewer than the requested number of objects (see the `count` query parameter documentation) or is empty.  ### opaquePassword  Attributes with the opaque property are also write-only and so cannot normally be retrieved in a GET. However, when a password is provided in the `opaquePassword` query parameter, attributes with the opaque property are retrieved in a GET in opaque form, encrypted with this password. The query parameter can also be used on a POST, PATCH, or PUT to set opaque attributes using opaque attribute values retrieved in a GET, so long as:  1. the same password that was used to retrieve the opaque attribute values is provided; and  2. the broker to which the request is being sent has the same major and minor SEMP version as the broker that produced the opaque attribute values.  The password provided in the query parameter must be a minimum of 8 characters and a maximum of 128 characters.  The query parameter can only be used in the configuration API, and only over HTTPS.  ## Authentication  When a client makes its first SEMPv2 request, it must supply a username and password using HTTP Basic authentication, or an OAuth token or tokens using HTTP Bearer authentication.  When HTTP Basic authentication is used, the broker returns a cookie containing a session key. The client can omit the username and password from subsequent requests, because the broker can use the session cookie for authentication instead. When the session expires or is deleted, the client must provide the username and password again, and the broker creates a new session.  There are a limited number of session slots available on the broker. The broker returns 529 No SEMP Session Available if it is not able to allocate a session.  If certain attributes—such as a user's password—are changed, the broker automatically deletes the affected sessions. These attributes are documented below. However, changes in external user configuration data stored on a RADIUS or LDAP server do not trigger the broker to delete the associated session(s), therefore you must do this manually, if required.  A client can retrieve its current session information using the /about/user endpoint and delete its own session using the /about/user/logout endpoint. A client with appropriate permissions can also manage all sessions using the /sessions endpoint.  Sessions are not created when authenticating with an OAuth token or tokens using HTTP Bearer authentication. If a session cookie is provided, it is ignored.  ## Help  Visit [our website](https://solace.com) to learn more about Solace.  You can also download the SEMP API specifications by clicking [here](https://solace.com/downloads/).  If you need additional support, please contact us at [support@solace.com](mailto:support@solace.com).  ## Notes  Note|Description :- --:|:- -- 1|This specification defines SEMP starting in \"v2\", and not the original SEMP \"v1\" interface. Request and response formats between \"v1\" and \"v2\" are entirely incompatible, although both protocols share a common port configuration on the Solace PubSub+ broker. They are differentiated by the initial portion of the URI path, one of either \"/SEMP/\" or \"/SEMP/v2/\" 2|This API is partially implemented. Only a subset of all objects are available. 3|Read-only attributes may appear in POST and PUT/PATCH requests. However, if a read-only attribute is not marked as identifying, it will be ignored during a PUT/PATCH. 4|On a PUT, if the SEMP user is not authorized to modify the attribute, its value is left unchanged rather than set to default. In addition, the values of write-only attributes are not set to their defaults on a PUT, except in the following two cases: there is a mutual requires relationship with another non-write-only attribute, both attributes are absent from the request, and the non-write-only attribute is not currently set to its default value; or the attribute is also opaque and the `opaquePassword` query parameter is provided in the request.  
 *
 * OpenAPI spec version: 2.35
 * Contact: support@solace.com
 * Generated by: https://github.com/swagger-api/swagger-codegen.git
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RestSharp.Portable;
using Semp.V2.CSharp.Client;
using Semp.V2.CSharp.Model;

namespace Semp.V2.CSharp.Api
{
    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public interface IOauthProfileApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Create an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Create an OAuth Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientSecret||||x||x oauthProfileName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResponse</returns>
        OauthProfileResponse CreateOauthProfile (OauthProfile body, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Create an OAuth Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientSecret||||x||x oauthProfileName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResponse</returns>
        ApiResponse<OauthProfileResponse> CreateOauthProfileWithHttpInfo (OauthProfile body, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Create a Group Access Level object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupResponse</returns>
        OauthProfileAccessLevelGroupResponse CreateOauthProfileAccessLevelGroup (OauthProfileAccessLevelGroup body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Create a Group Access Level object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupResponse</returns>
        ApiResponse<OauthProfileAccessLevelGroupResponse> CreateOauthProfileAccessLevelGroupWithHttpInfo (OauthProfileAccessLevelGroup body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x||x||| msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x||x||| msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create an Allowed Host Value object.
        /// </summary>
        /// <remarks>
        /// Create an Allowed Host Value object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: allowedHost|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Allowed Host Value object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAllowedHostResponse</returns>
        OauthProfileClientAllowedHostResponse CreateOauthProfileClientAllowedHost (OauthProfileClientAllowedHost body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create an Allowed Host Value object.
        /// </summary>
        /// <remarks>
        /// Create an Allowed Host Value object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: allowedHost|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Allowed Host Value object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAllowedHostResponse</returns>
        ApiResponse<OauthProfileClientAllowedHostResponse> CreateOauthProfileClientAllowedHostWithHttpInfo (OauthProfileClientAllowedHost body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Create an Authorization Parameter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAuthorizationParameterResponse</returns>
        OauthProfileClientAuthorizationParameterResponse CreateOauthProfileClientAuthorizationParameter (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Create an Authorization Parameter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAuthorizationParameterResponse</returns>
        ApiResponse<OauthProfileClientAuthorizationParameterResponse> CreateOauthProfileClientAuthorizationParameterWithHttpInfo (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x|x|||| clientRequiredClaimValue||x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientRequiredClaimResponse</returns>
        OauthProfileClientRequiredClaimResponse CreateOauthProfileClientRequiredClaim (OauthProfileClientRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x|x|||| clientRequiredClaimValue||x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientRequiredClaimResponse</returns>
        ApiResponse<OauthProfileClientRequiredClaimResponse> CreateOauthProfileClientRequiredClaimWithHttpInfo (OauthProfileClientRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        OauthProfileDefaultMsgVpnAccessLevelExceptionResponse CreateOauthProfileDefaultMsgVpnAccessLevelException (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> CreateOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||x||| resourceServerRequiredClaimName|x|x|||| resourceServerRequiredClaimValue||x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResourceServerRequiredClaimResponse</returns>
        OauthProfileResourceServerRequiredClaimResponse CreateOauthProfileResourceServerRequiredClaim (OauthProfileResourceServerRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||x||| resourceServerRequiredClaimName|x|x|||| resourceServerRequiredClaimValue||x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResourceServerRequiredClaimResponse</returns>
        ApiResponse<OauthProfileResourceServerRequiredClaimResponse> CreateOauthProfileResourceServerRequiredClaimWithHttpInfo (OauthProfileResourceServerRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Delete an OAuth Profile object. The deletion of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteOauthProfile (string oauthProfileName);

        /// <summary>
        /// Delete an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Delete an OAuth Profile object. The deletion of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteOauthProfileWithHttpInfo (string oauthProfileName);
        /// <summary>
        /// Delete a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Delete a Group Access Level object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteOauthProfileAccessLevelGroup (string oauthProfileName, string groupName);

        /// <summary>
        /// Delete a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Delete a Group Access Level object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteOauthProfileAccessLevelGroupWithHttpInfo (string oauthProfileName, string groupName);
        /// <summary>
        /// Delete a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException (string oauthProfileName, string groupName, string msgVpnName);

        /// <summary>
        /// Delete a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo (string oauthProfileName, string groupName, string msgVpnName);
        /// <summary>
        /// Delete an Allowed Host Value object.
        /// </summary>
        /// <remarks>
        /// Delete an Allowed Host Value object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteOauthProfileClientAllowedHost (string oauthProfileName, string allowedHost);

        /// <summary>
        /// Delete an Allowed Host Value object.
        /// </summary>
        /// <remarks>
        /// Delete an Allowed Host Value object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteOauthProfileClientAllowedHostWithHttpInfo (string oauthProfileName, string allowedHost);
        /// <summary>
        /// Delete an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Delete an Authorization Parameter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteOauthProfileClientAuthorizationParameter (string oauthProfileName, string authorizationParameterName);

        /// <summary>
        /// Delete an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Delete an Authorization Parameter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteOauthProfileClientAuthorizationParameterWithHttpInfo (string oauthProfileName, string authorizationParameterName);
        /// <summary>
        /// Delete a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteOauthProfileClientRequiredClaim (string oauthProfileName, string clientRequiredClaimName);

        /// <summary>
        /// Delete a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteOauthProfileClientRequiredClaimWithHttpInfo (string oauthProfileName, string clientRequiredClaimName);
        /// <summary>
        /// Delete a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteOauthProfileDefaultMsgVpnAccessLevelException (string oauthProfileName, string msgVpnName);

        /// <summary>
        /// Delete a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo (string oauthProfileName, string msgVpnName);
        /// <summary>
        /// Delete a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteOauthProfileResourceServerRequiredClaim (string oauthProfileName, string resourceServerRequiredClaimName);

        /// <summary>
        /// Delete a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteOauthProfileResourceServerRequiredClaimWithHttpInfo (string oauthProfileName, string resourceServerRequiredClaimName);
        /// <summary>
        /// Get an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Get an OAuth Profile object.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResponse</returns>
        OauthProfileResponse GetOauthProfile (string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Get an OAuth Profile object.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResponse</returns>
        ApiResponse<OauthProfileResponse> GetOauthProfileWithHttpInfo (string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Get a Group Access Level object.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupResponse</returns>
        OauthProfileAccessLevelGroupResponse GetOauthProfileAccessLevelGroup (string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Get a Group Access Level object.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupResponse</returns>
        ApiResponse<OauthProfileAccessLevelGroupResponse> GetOauthProfileAccessLevelGroupWithHttpInfo (string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Message VPN Access-Level Exception object.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException (string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Message VPN Access-Level Exception object.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo (string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Message VPN Access-Level Exception objects.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse</returns>
        OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions (string oauthProfileName, string groupName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Message VPN Access-Level Exception objects.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse</returns>
        ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse> GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsWithHttpInfo (string oauthProfileName, string groupName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Group Access Level objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Group Access Level objects.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupsResponse</returns>
        OauthProfileAccessLevelGroupsResponse GetOauthProfileAccessLevelGroups (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Group Access Level objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Group Access Level objects.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupsResponse</returns>
        ApiResponse<OauthProfileAccessLevelGroupsResponse> GetOauthProfileAccessLevelGroupsWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get an Allowed Host Value object.
        /// </summary>
        /// <remarks>
        /// Get an Allowed Host Value object.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAllowedHostResponse</returns>
        OauthProfileClientAllowedHostResponse GetOauthProfileClientAllowedHost (string oauthProfileName, string allowedHost, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get an Allowed Host Value object.
        /// </summary>
        /// <remarks>
        /// Get an Allowed Host Value object.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAllowedHostResponse</returns>
        ApiResponse<OauthProfileClientAllowedHostResponse> GetOauthProfileClientAllowedHostWithHttpInfo (string oauthProfileName, string allowedHost, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Allowed Host Value objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Allowed Host Value objects.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAllowedHostsResponse</returns>
        OauthProfileClientAllowedHostsResponse GetOauthProfileClientAllowedHosts (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Allowed Host Value objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Allowed Host Value objects.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAllowedHostsResponse</returns>
        ApiResponse<OauthProfileClientAllowedHostsResponse> GetOauthProfileClientAllowedHostsWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Get an Authorization Parameter object.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAuthorizationParameterResponse</returns>
        OauthProfileClientAuthorizationParameterResponse GetOauthProfileClientAuthorizationParameter (string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Get an Authorization Parameter object.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAuthorizationParameterResponse</returns>
        ApiResponse<OauthProfileClientAuthorizationParameterResponse> GetOauthProfileClientAuthorizationParameterWithHttpInfo (string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Authorization Parameter objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Authorization Parameter objects.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAuthorizationParametersResponse</returns>
        OauthProfileClientAuthorizationParametersResponse GetOauthProfileClientAuthorizationParameters (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Authorization Parameter objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Authorization Parameter objects.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAuthorizationParametersResponse</returns>
        ApiResponse<OauthProfileClientAuthorizationParametersResponse> GetOauthProfileClientAuthorizationParametersWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Get a Required Claim object.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientRequiredClaimResponse</returns>
        OauthProfileClientRequiredClaimResponse GetOauthProfileClientRequiredClaim (string oauthProfileName, string clientRequiredClaimName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Get a Required Claim object.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientRequiredClaimResponse</returns>
        ApiResponse<OauthProfileClientRequiredClaimResponse> GetOauthProfileClientRequiredClaimWithHttpInfo (string oauthProfileName, string clientRequiredClaimName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Required Claim objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Required Claim objects.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientRequiredClaimsResponse</returns>
        OauthProfileClientRequiredClaimsResponse GetOauthProfileClientRequiredClaims (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Required Claim objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Required Claim objects.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientRequiredClaimsResponse</returns>
        ApiResponse<OauthProfileClientRequiredClaimsResponse> GetOauthProfileClientRequiredClaimsWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Message VPN Access-Level Exception object.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        OauthProfileDefaultMsgVpnAccessLevelExceptionResponse GetOauthProfileDefaultMsgVpnAccessLevelException (string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Message VPN Access-Level Exception object.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> GetOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo (string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Message VPN Access-Level Exception objects.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse</returns>
        OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse GetOauthProfileDefaultMsgVpnAccessLevelExceptions (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Message VPN Access-Level Exception objects.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse</returns>
        ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse> GetOauthProfileDefaultMsgVpnAccessLevelExceptionsWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Get a Required Claim object.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResourceServerRequiredClaimResponse</returns>
        OauthProfileResourceServerRequiredClaimResponse GetOauthProfileResourceServerRequiredClaim (string oauthProfileName, string resourceServerRequiredClaimName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Get a Required Claim object.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResourceServerRequiredClaimResponse</returns>
        ApiResponse<OauthProfileResourceServerRequiredClaimResponse> GetOauthProfileResourceServerRequiredClaimWithHttpInfo (string oauthProfileName, string resourceServerRequiredClaimName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Required Claim objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Required Claim objects.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResourceServerRequiredClaimsResponse</returns>
        OauthProfileResourceServerRequiredClaimsResponse GetOauthProfileResourceServerRequiredClaims (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Required Claim objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Required Claim objects.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResourceServerRequiredClaimsResponse</returns>
        ApiResponse<OauthProfileResourceServerRequiredClaimsResponse> GetOauthProfileResourceServerRequiredClaimsWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of OAuth Profile objects.
        /// </summary>
        /// <remarks>
        /// Get a list of OAuth Profile objects.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfilesResponse</returns>
        OauthProfilesResponse GetOauthProfiles (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of OAuth Profile objects.
        /// </summary>
        /// <remarks>
        /// Get a list of OAuth Profile objects.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfilesResponse</returns>
        ApiResponse<OauthProfilesResponse> GetOauthProfilesWithHttpInfo (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Replace an OAuth Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret||||x||||x oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResponse</returns>
        OauthProfileResponse ReplaceOauthProfile (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Replace an OAuth Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret||||x||||x oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResponse</returns>
        ApiResponse<OauthProfileResponse> ReplaceOauthProfileWithHttpInfo (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Replace a Group Access Level object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupResponse</returns>
        OauthProfileAccessLevelGroupResponse ReplaceOauthProfileAccessLevelGroup (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Replace a Group Access Level object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupResponse</returns>
        ApiResponse<OauthProfileAccessLevelGroupResponse> ReplaceOauthProfileAccessLevelGroupWithHttpInfo (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Replace an Authorization Parameter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAuthorizationParameterResponse</returns>
        OauthProfileClientAuthorizationParameterResponse ReplaceOauthProfileClientAuthorizationParameter (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Replace an Authorization Parameter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAuthorizationParameterResponse</returns>
        ApiResponse<OauthProfileClientAuthorizationParameterResponse> ReplaceOauthProfileClientAuthorizationParameterWithHttpInfo (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Default message VPN access-level exceptions.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        OauthProfileDefaultMsgVpnAccessLevelExceptionResponse ReplaceOauthProfileDefaultMsgVpnAccessLevelException (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Default message VPN access-level exceptions.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> ReplaceOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Update an OAuth Profile object. Any attribute missing from the request will be left unchanged.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret|||x||||x oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResponse</returns>
        OauthProfileResponse UpdateOauthProfile (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Update an OAuth Profile object. Any attribute missing from the request will be left unchanged.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret|||x||||x oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResponse</returns>
        ApiResponse<OauthProfileResponse> UpdateOauthProfileWithHttpInfo (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Update a Group Access Level object. Any attribute missing from the request will be left unchanged.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupResponse</returns>
        OauthProfileAccessLevelGroupResponse UpdateOauthProfileAccessLevelGroup (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Update a Group Access Level object. Any attribute missing from the request will be left unchanged.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupResponse</returns>
        ApiResponse<OauthProfileAccessLevelGroupResponse> UpdateOauthProfileAccessLevelGroupWithHttpInfo (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Update an Authorization Parameter object. Any attribute missing from the request will be left unchanged.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAuthorizationParameterResponse</returns>
        OauthProfileClientAuthorizationParameterResponse UpdateOauthProfileClientAuthorizationParameter (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Update an Authorization Parameter object. Any attribute missing from the request will be left unchanged.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAuthorizationParameterResponse</returns>
        ApiResponse<OauthProfileClientAuthorizationParameterResponse> UpdateOauthProfileClientAuthorizationParameterWithHttpInfo (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Default message VPN access-level exceptions.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        OauthProfileDefaultMsgVpnAccessLevelExceptionResponse UpdateOauthProfileDefaultMsgVpnAccessLevelException (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Default message VPN access-level exceptions.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> UpdateOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// Create an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Create an OAuth Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientSecret||||x||x oauthProfileName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResponse</returns>
        System.Threading.Tasks.Task<OauthProfileResponse> CreateOauthProfileAsync (OauthProfile body, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Create an OAuth Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientSecret||||x||x oauthProfileName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileResponse>> CreateOauthProfileAsyncWithHttpInfo (OauthProfile body, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Create a Group Access Level object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupResponse</returns>
        System.Threading.Tasks.Task<OauthProfileAccessLevelGroupResponse> CreateOauthProfileAccessLevelGroupAsync (OauthProfileAccessLevelGroup body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Create a Group Access Level object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupResponse>> CreateOauthProfileAccessLevelGroupAsyncWithHttpInfo (OauthProfileAccessLevelGroup body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x||x||| msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        System.Threading.Tasks.Task<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsync (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x||x||| msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>> CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create an Allowed Host Value object.
        /// </summary>
        /// <remarks>
        /// Create an Allowed Host Value object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: allowedHost|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Allowed Host Value object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAllowedHostResponse</returns>
        System.Threading.Tasks.Task<OauthProfileClientAllowedHostResponse> CreateOauthProfileClientAllowedHostAsync (OauthProfileClientAllowedHost body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create an Allowed Host Value object.
        /// </summary>
        /// <remarks>
        /// Create an Allowed Host Value object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: allowedHost|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Allowed Host Value object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAllowedHostResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAllowedHostResponse>> CreateOauthProfileClientAllowedHostAsyncWithHttpInfo (OauthProfileClientAllowedHost body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Create an Authorization Parameter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAuthorizationParameterResponse</returns>
        System.Threading.Tasks.Task<OauthProfileClientAuthorizationParameterResponse> CreateOauthProfileClientAuthorizationParameterAsync (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Create an Authorization Parameter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAuthorizationParameterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAuthorizationParameterResponse>> CreateOauthProfileClientAuthorizationParameterAsyncWithHttpInfo (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x|x|||| clientRequiredClaimValue||x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientRequiredClaimResponse</returns>
        System.Threading.Tasks.Task<OauthProfileClientRequiredClaimResponse> CreateOauthProfileClientRequiredClaimAsync (OauthProfileClientRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x|x|||| clientRequiredClaimValue||x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientRequiredClaimResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileClientRequiredClaimResponse>> CreateOauthProfileClientRequiredClaimAsyncWithHttpInfo (OauthProfileClientRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        System.Threading.Tasks.Task<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> CreateOauthProfileDefaultMsgVpnAccessLevelExceptionAsync (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>> CreateOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||x||| resourceServerRequiredClaimName|x|x|||| resourceServerRequiredClaimValue||x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResourceServerRequiredClaimResponse</returns>
        System.Threading.Tasks.Task<OauthProfileResourceServerRequiredClaimResponse> CreateOauthProfileResourceServerRequiredClaimAsync (OauthProfileResourceServerRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||x||| resourceServerRequiredClaimName|x|x|||| resourceServerRequiredClaimValue||x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResourceServerRequiredClaimResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileResourceServerRequiredClaimResponse>> CreateOauthProfileResourceServerRequiredClaimAsyncWithHttpInfo (OauthProfileResourceServerRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Delete an OAuth Profile object. The deletion of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileAsync (string oauthProfileName);

        /// <summary>
        /// Delete an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Delete an OAuth Profile object. The deletion of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileAsyncWithHttpInfo (string oauthProfileName);
        /// <summary>
        /// Delete a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Delete a Group Access Level object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileAccessLevelGroupAsync (string oauthProfileName, string groupName);

        /// <summary>
        /// Delete a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Delete a Group Access Level object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileAccessLevelGroupAsyncWithHttpInfo (string oauthProfileName, string groupName);
        /// <summary>
        /// Delete a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsync (string oauthProfileName, string groupName, string msgVpnName);

        /// <summary>
        /// Delete a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo (string oauthProfileName, string groupName, string msgVpnName);
        /// <summary>
        /// Delete an Allowed Host Value object.
        /// </summary>
        /// <remarks>
        /// Delete an Allowed Host Value object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileClientAllowedHostAsync (string oauthProfileName, string allowedHost);

        /// <summary>
        /// Delete an Allowed Host Value object.
        /// </summary>
        /// <remarks>
        /// Delete an Allowed Host Value object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileClientAllowedHostAsyncWithHttpInfo (string oauthProfileName, string allowedHost);
        /// <summary>
        /// Delete an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Delete an Authorization Parameter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileClientAuthorizationParameterAsync (string oauthProfileName, string authorizationParameterName);

        /// <summary>
        /// Delete an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Delete an Authorization Parameter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileClientAuthorizationParameterAsyncWithHttpInfo (string oauthProfileName, string authorizationParameterName);
        /// <summary>
        /// Delete a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileClientRequiredClaimAsync (string oauthProfileName, string clientRequiredClaimName);

        /// <summary>
        /// Delete a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileClientRequiredClaimAsyncWithHttpInfo (string oauthProfileName, string clientRequiredClaimName);
        /// <summary>
        /// Delete a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileDefaultMsgVpnAccessLevelExceptionAsync (string oauthProfileName, string msgVpnName);

        /// <summary>
        /// Delete a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo (string oauthProfileName, string msgVpnName);
        /// <summary>
        /// Delete a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileResourceServerRequiredClaimAsync (string oauthProfileName, string resourceServerRequiredClaimName);

        /// <summary>
        /// Delete a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileResourceServerRequiredClaimAsyncWithHttpInfo (string oauthProfileName, string resourceServerRequiredClaimName);
        /// <summary>
        /// Get an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Get an OAuth Profile object.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResponse</returns>
        System.Threading.Tasks.Task<OauthProfileResponse> GetOauthProfileAsync (string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Get an OAuth Profile object.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileResponse>> GetOauthProfileAsyncWithHttpInfo (string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Get a Group Access Level object.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupResponse</returns>
        System.Threading.Tasks.Task<OauthProfileAccessLevelGroupResponse> GetOauthProfileAccessLevelGroupAsync (string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Get a Group Access Level object.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupResponse>> GetOauthProfileAccessLevelGroupAsyncWithHttpInfo (string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Message VPN Access-Level Exception object.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        System.Threading.Tasks.Task<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsync (string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Message VPN Access-Level Exception object.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>> GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo (string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Message VPN Access-Level Exception objects.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse</returns>
        System.Threading.Tasks.Task<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse> GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsAsync (string oauthProfileName, string groupName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Message VPN Access-Level Exception objects.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse>> GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsAsyncWithHttpInfo (string oauthProfileName, string groupName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Group Access Level objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Group Access Level objects.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupsResponse</returns>
        System.Threading.Tasks.Task<OauthProfileAccessLevelGroupsResponse> GetOauthProfileAccessLevelGroupsAsync (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Group Access Level objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Group Access Level objects.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupsResponse>> GetOauthProfileAccessLevelGroupsAsyncWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get an Allowed Host Value object.
        /// </summary>
        /// <remarks>
        /// Get an Allowed Host Value object.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAllowedHostResponse</returns>
        System.Threading.Tasks.Task<OauthProfileClientAllowedHostResponse> GetOauthProfileClientAllowedHostAsync (string oauthProfileName, string allowedHost, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get an Allowed Host Value object.
        /// </summary>
        /// <remarks>
        /// Get an Allowed Host Value object.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAllowedHostResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAllowedHostResponse>> GetOauthProfileClientAllowedHostAsyncWithHttpInfo (string oauthProfileName, string allowedHost, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Allowed Host Value objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Allowed Host Value objects.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAllowedHostsResponse</returns>
        System.Threading.Tasks.Task<OauthProfileClientAllowedHostsResponse> GetOauthProfileClientAllowedHostsAsync (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Allowed Host Value objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Allowed Host Value objects.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAllowedHostsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAllowedHostsResponse>> GetOauthProfileClientAllowedHostsAsyncWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Get an Authorization Parameter object.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAuthorizationParameterResponse</returns>
        System.Threading.Tasks.Task<OauthProfileClientAuthorizationParameterResponse> GetOauthProfileClientAuthorizationParameterAsync (string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Get an Authorization Parameter object.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAuthorizationParameterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAuthorizationParameterResponse>> GetOauthProfileClientAuthorizationParameterAsyncWithHttpInfo (string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Authorization Parameter objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Authorization Parameter objects.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAuthorizationParametersResponse</returns>
        System.Threading.Tasks.Task<OauthProfileClientAuthorizationParametersResponse> GetOauthProfileClientAuthorizationParametersAsync (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Authorization Parameter objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Authorization Parameter objects.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAuthorizationParametersResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAuthorizationParametersResponse>> GetOauthProfileClientAuthorizationParametersAsyncWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Get a Required Claim object.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientRequiredClaimResponse</returns>
        System.Threading.Tasks.Task<OauthProfileClientRequiredClaimResponse> GetOauthProfileClientRequiredClaimAsync (string oauthProfileName, string clientRequiredClaimName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Get a Required Claim object.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientRequiredClaimResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileClientRequiredClaimResponse>> GetOauthProfileClientRequiredClaimAsyncWithHttpInfo (string oauthProfileName, string clientRequiredClaimName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Required Claim objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Required Claim objects.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientRequiredClaimsResponse</returns>
        System.Threading.Tasks.Task<OauthProfileClientRequiredClaimsResponse> GetOauthProfileClientRequiredClaimsAsync (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Required Claim objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Required Claim objects.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientRequiredClaimsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileClientRequiredClaimsResponse>> GetOauthProfileClientRequiredClaimsAsyncWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Message VPN Access-Level Exception object.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        System.Threading.Tasks.Task<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> GetOauthProfileDefaultMsgVpnAccessLevelExceptionAsync (string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Message VPN Access-Level Exception object.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>> GetOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo (string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Message VPN Access-Level Exception objects.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse</returns>
        System.Threading.Tasks.Task<OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse> GetOauthProfileDefaultMsgVpnAccessLevelExceptionsAsync (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Message VPN Access-Level Exception objects.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse>> GetOauthProfileDefaultMsgVpnAccessLevelExceptionsAsyncWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Get a Required Claim object.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResourceServerRequiredClaimResponse</returns>
        System.Threading.Tasks.Task<OauthProfileResourceServerRequiredClaimResponse> GetOauthProfileResourceServerRequiredClaimAsync (string oauthProfileName, string resourceServerRequiredClaimName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Required Claim object.
        /// </summary>
        /// <remarks>
        /// Get a Required Claim object.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResourceServerRequiredClaimResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileResourceServerRequiredClaimResponse>> GetOauthProfileResourceServerRequiredClaimAsyncWithHttpInfo (string oauthProfileName, string resourceServerRequiredClaimName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Required Claim objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Required Claim objects.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResourceServerRequiredClaimsResponse</returns>
        System.Threading.Tasks.Task<OauthProfileResourceServerRequiredClaimsResponse> GetOauthProfileResourceServerRequiredClaimsAsync (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Required Claim objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Required Claim objects.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResourceServerRequiredClaimsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileResourceServerRequiredClaimsResponse>> GetOauthProfileResourceServerRequiredClaimsAsyncWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of OAuth Profile objects.
        /// </summary>
        /// <remarks>
        /// Get a list of OAuth Profile objects.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfilesResponse</returns>
        System.Threading.Tasks.Task<OauthProfilesResponse> GetOauthProfilesAsync (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of OAuth Profile objects.
        /// </summary>
        /// <remarks>
        /// Get a list of OAuth Profile objects.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfilesResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfilesResponse>> GetOauthProfilesAsyncWithHttpInfo (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Replace an OAuth Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret||||x||||x oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResponse</returns>
        System.Threading.Tasks.Task<OauthProfileResponse> ReplaceOauthProfileAsync (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Replace an OAuth Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret||||x||||x oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileResponse>> ReplaceOauthProfileAsyncWithHttpInfo (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Replace a Group Access Level object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupResponse</returns>
        System.Threading.Tasks.Task<OauthProfileAccessLevelGroupResponse> ReplaceOauthProfileAccessLevelGroupAsync (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Replace a Group Access Level object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupResponse>> ReplaceOauthProfileAccessLevelGroupAsyncWithHttpInfo (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        System.Threading.Tasks.Task<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsync (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>> ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Replace an Authorization Parameter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAuthorizationParameterResponse</returns>
        System.Threading.Tasks.Task<OauthProfileClientAuthorizationParameterResponse> ReplaceOauthProfileClientAuthorizationParameterAsync (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Replace an Authorization Parameter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAuthorizationParameterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAuthorizationParameterResponse>> ReplaceOauthProfileClientAuthorizationParameterAsyncWithHttpInfo (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Default message VPN access-level exceptions.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        System.Threading.Tasks.Task<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> ReplaceOauthProfileDefaultMsgVpnAccessLevelExceptionAsync (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Default message VPN access-level exceptions.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>> ReplaceOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Update an OAuth Profile object. Any attribute missing from the request will be left unchanged.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret|||x||||x oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResponse</returns>
        System.Threading.Tasks.Task<OauthProfileResponse> UpdateOauthProfileAsync (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update an OAuth Profile object.
        /// </summary>
        /// <remarks>
        /// Update an OAuth Profile object. Any attribute missing from the request will be left unchanged.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret|||x||||x oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileResponse>> UpdateOauthProfileAsyncWithHttpInfo (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Update a Group Access Level object. Any attribute missing from the request will be left unchanged.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupResponse</returns>
        System.Threading.Tasks.Task<OauthProfileAccessLevelGroupResponse> UpdateOauthProfileAccessLevelGroupAsync (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Group Access Level object.
        /// </summary>
        /// <remarks>
        /// Update a Group Access Level object. Any attribute missing from the request will be left unchanged.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupResponse>> UpdateOauthProfileAccessLevelGroupAsyncWithHttpInfo (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        System.Threading.Tasks.Task<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsync (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>> UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Update an Authorization Parameter object. Any attribute missing from the request will be left unchanged.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAuthorizationParameterResponse</returns>
        System.Threading.Tasks.Task<OauthProfileClientAuthorizationParameterResponse> UpdateOauthProfileClientAuthorizationParameterAsync (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update an Authorization Parameter object.
        /// </summary>
        /// <remarks>
        /// Update an Authorization Parameter object. Any attribute missing from the request will be left unchanged.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAuthorizationParameterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAuthorizationParameterResponse>> UpdateOauthProfileClientAuthorizationParameterAsyncWithHttpInfo (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Default message VPN access-level exceptions.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        System.Threading.Tasks.Task<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> UpdateOauthProfileDefaultMsgVpnAccessLevelExceptionAsync (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Message VPN Access-Level Exception object.
        /// </summary>
        /// <remarks>
        /// Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Default message VPN access-level exceptions.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>> UpdateOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public partial class OauthProfileApi : IOauthProfileApi
    {
        private Semp.V2.CSharp.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="OauthProfileApi"/> class.
        /// </summary>
        /// <returns></returns>
        public OauthProfileApi(String basePath)
        {
            this.Configuration = new Semp.V2.CSharp.Client.Configuration { BasePath = basePath };

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OauthProfileApi"/> class
        /// </summary>
        /// <returns></returns>
        public OauthProfileApi()
        {
            this.Configuration = Semp.V2.CSharp.Client.Configuration.Default;

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OauthProfileApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public OauthProfileApi(Semp.V2.CSharp.Client.Configuration configuration = null)
        {
            if (configuration == null) // use the default one in Configuration
                this.Configuration = Semp.V2.CSharp.Client.Configuration.Default;
            else
                this.Configuration = configuration;

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Gets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        public String GetBasePath()
        {
            return this.Configuration.ApiClient.RestClient.BaseUrl.ToString();
        }

        /// <summary>
        /// Sets the base path of the API client.
        /// </summary>
        /// <value>The base path</value>
        [Obsolete("SetBasePath is deprecated, please do 'Configuration.ApiClient = new ApiClient(\"http://new-path\")' instead.")]
        public void SetBasePath(String basePath)
        {
            // do nothing
        }

        /// <summary>
        /// Gets or sets the configuration object
        /// </summary>
        /// <value>An instance of the Configuration</value>
        public Semp.V2.CSharp.Client.Configuration Configuration {get; set;}

        /// <summary>
        /// Provides a factory method hook for the creation of exceptions.
        /// </summary>
        public Semp.V2.CSharp.Client.ExceptionFactory ExceptionFactory
        {
            get
            {
                if (_exceptionFactory != null && _exceptionFactory.GetInvocationList().Length > 1)
                {
                    throw new InvalidOperationException("Multicast delegate for ExceptionFactory is unsupported.");
                }
                return _exceptionFactory;
            }
            set { _exceptionFactory = value; }
        }

        /// <summary>
        /// Gets the default header.
        /// </summary>
        /// <returns>Dictionary of HTTP header</returns>
        [Obsolete("DefaultHeader is deprecated, please use Configuration.DefaultHeader instead.")]
        public IDictionary<String, String> DefaultHeader()
        {
            return new ReadOnlyDictionary<string, string>(this.Configuration.DefaultHeader);
        }

        /// <summary>
        /// Add default header.
        /// </summary>
        /// <param name="key">Header field name.</param>
        /// <param name="value">Header field value.</param>
        /// <returns></returns>
        [Obsolete("AddDefaultHeader is deprecated, please use Configuration.AddDefaultHeader instead.")]
        public void AddDefaultHeader(string key, string value)
        {
            this.Configuration.AddDefaultHeader(key, value);
        }

        /// <summary>
        /// Create an OAuth Profile object. Create an OAuth Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientSecret||||x||x oauthProfileName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResponse</returns>
        public OauthProfileResponse CreateOauthProfile (OauthProfile body, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResponse> localVarResponse = CreateOauthProfileWithHttpInfo(body, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create an OAuth Profile object. Create an OAuth Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientSecret||||x||x oauthProfileName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResponse</returns>
        public ApiResponse< OauthProfileResponse > CreateOauthProfileWithHttpInfo (OauthProfile body, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfile");

            var localVarPath = "./oauthProfiles";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResponse)));
        }

        /// <summary>
        /// Create an OAuth Profile object. Create an OAuth Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientSecret||||x||x oauthProfileName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileResponse> CreateOauthProfileAsync (OauthProfile body, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResponse> localVarResponse = await CreateOauthProfileAsyncWithHttpInfo(body, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create an OAuth Profile object. Create an OAuth Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientSecret||||x||x oauthProfileName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileResponse>> CreateOauthProfileAsyncWithHttpInfo (OauthProfile body, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfile");

            var localVarPath = "./oauthProfiles";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResponse)));
        }

        /// <summary>
        /// Create a Group Access Level object. Create a Group Access Level object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupResponse</returns>
        public OauthProfileAccessLevelGroupResponse CreateOauthProfileAccessLevelGroup (OauthProfileAccessLevelGroup body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupResponse> localVarResponse = CreateOauthProfileAccessLevelGroupWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Group Access Level object. Create a Group Access Level object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupResponse</returns>
        public ApiResponse< OauthProfileAccessLevelGroupResponse > CreateOauthProfileAccessLevelGroupWithHttpInfo (OauthProfileAccessLevelGroup body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileAccessLevelGroup");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileAccessLevelGroup");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileAccessLevelGroup", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupResponse)));
        }

        /// <summary>
        /// Create a Group Access Level object. Create a Group Access Level object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileAccessLevelGroupResponse> CreateOauthProfileAccessLevelGroupAsync (OauthProfileAccessLevelGroup body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupResponse> localVarResponse = await CreateOauthProfileAccessLevelGroupAsyncWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Group Access Level object. Create a Group Access Level object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupResponse>> CreateOauthProfileAccessLevelGroupAsyncWithHttpInfo (OauthProfileAccessLevelGroup body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileAccessLevelGroup");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileAccessLevelGroup");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileAccessLevelGroup", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupResponse)));
        }

        /// <summary>
        /// Create a Message VPN Access-Level Exception object. Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x||x||| msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        public OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> localVarResponse = CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo(body, oauthProfileName, groupName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Message VPN Access-Level Exception object. Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x||x||| msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        public ApiResponse< OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse > CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Create a Message VPN Access-Level Exception object. Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x||x||| msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsync (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> localVarResponse = await CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo(body, oauthProfileName, groupName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Message VPN Access-Level Exception object. Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: groupName|x||x||| msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>> CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileAccessLevelGroupMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Create an Allowed Host Value object. Create an Allowed Host Value object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: allowedHost|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Allowed Host Value object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAllowedHostResponse</returns>
        public OauthProfileClientAllowedHostResponse CreateOauthProfileClientAllowedHost (OauthProfileClientAllowedHost body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAllowedHostResponse> localVarResponse = CreateOauthProfileClientAllowedHostWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create an Allowed Host Value object. Create an Allowed Host Value object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: allowedHost|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Allowed Host Value object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAllowedHostResponse</returns>
        public ApiResponse< OauthProfileClientAllowedHostResponse > CreateOauthProfileClientAllowedHostWithHttpInfo (OauthProfileClientAllowedHost body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileClientAllowedHost");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileClientAllowedHost");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAllowedHosts";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileClientAllowedHost", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAllowedHostResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAllowedHostResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAllowedHostResponse)));
        }

        /// <summary>
        /// Create an Allowed Host Value object. Create an Allowed Host Value object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: allowedHost|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Allowed Host Value object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAllowedHostResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileClientAllowedHostResponse> CreateOauthProfileClientAllowedHostAsync (OauthProfileClientAllowedHost body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAllowedHostResponse> localVarResponse = await CreateOauthProfileClientAllowedHostAsyncWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create an Allowed Host Value object. Create an Allowed Host Value object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: allowedHost|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Allowed Host Value object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAllowedHostResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAllowedHostResponse>> CreateOauthProfileClientAllowedHostAsyncWithHttpInfo (OauthProfileClientAllowedHost body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileClientAllowedHost");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileClientAllowedHost");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAllowedHosts";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileClientAllowedHost", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAllowedHostResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAllowedHostResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAllowedHostResponse)));
        }

        /// <summary>
        /// Create an Authorization Parameter object. Create an Authorization Parameter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAuthorizationParameterResponse</returns>
        public OauthProfileClientAuthorizationParameterResponse CreateOauthProfileClientAuthorizationParameter (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAuthorizationParameterResponse> localVarResponse = CreateOauthProfileClientAuthorizationParameterWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create an Authorization Parameter object. Create an Authorization Parameter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAuthorizationParameterResponse</returns>
        public ApiResponse< OauthProfileClientAuthorizationParameterResponse > CreateOauthProfileClientAuthorizationParameterWithHttpInfo (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileClientAuthorizationParameter");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAuthorizationParameters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileClientAuthorizationParameter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAuthorizationParameterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAuthorizationParameterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAuthorizationParameterResponse)));
        }

        /// <summary>
        /// Create an Authorization Parameter object. Create an Authorization Parameter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAuthorizationParameterResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileClientAuthorizationParameterResponse> CreateOauthProfileClientAuthorizationParameterAsync (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAuthorizationParameterResponse> localVarResponse = await CreateOauthProfileClientAuthorizationParameterAsyncWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create an Authorization Parameter object. Create an Authorization Parameter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAuthorizationParameterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAuthorizationParameterResponse>> CreateOauthProfileClientAuthorizationParameterAsyncWithHttpInfo (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileClientAuthorizationParameter");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAuthorizationParameters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileClientAuthorizationParameter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAuthorizationParameterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAuthorizationParameterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAuthorizationParameterResponse)));
        }

        /// <summary>
        /// Create a Required Claim object. Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x|x|||| clientRequiredClaimValue||x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientRequiredClaimResponse</returns>
        public OauthProfileClientRequiredClaimResponse CreateOauthProfileClientRequiredClaim (OauthProfileClientRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientRequiredClaimResponse> localVarResponse = CreateOauthProfileClientRequiredClaimWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Required Claim object. Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x|x|||| clientRequiredClaimValue||x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientRequiredClaimResponse</returns>
        public ApiResponse< OauthProfileClientRequiredClaimResponse > CreateOauthProfileClientRequiredClaimWithHttpInfo (OauthProfileClientRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileClientRequiredClaim");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileClientRequiredClaim");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientRequiredClaims";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileClientRequiredClaim", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientRequiredClaimResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientRequiredClaimResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientRequiredClaimResponse)));
        }

        /// <summary>
        /// Create a Required Claim object. Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x|x|||| clientRequiredClaimValue||x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientRequiredClaimResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileClientRequiredClaimResponse> CreateOauthProfileClientRequiredClaimAsync (OauthProfileClientRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientRequiredClaimResponse> localVarResponse = await CreateOauthProfileClientRequiredClaimAsyncWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Required Claim object. Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x|x|||| clientRequiredClaimValue||x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientRequiredClaimResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileClientRequiredClaimResponse>> CreateOauthProfileClientRequiredClaimAsyncWithHttpInfo (OauthProfileClientRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileClientRequiredClaim");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileClientRequiredClaim");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientRequiredClaims";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileClientRequiredClaim", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientRequiredClaimResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientRequiredClaimResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientRequiredClaimResponse)));
        }

        /// <summary>
        /// Create a Message VPN Access-Level Exception object. Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        public OauthProfileDefaultMsgVpnAccessLevelExceptionResponse CreateOauthProfileDefaultMsgVpnAccessLevelException (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> localVarResponse = CreateOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Message VPN Access-Level Exception object. Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        public ApiResponse< OauthProfileDefaultMsgVpnAccessLevelExceptionResponse > CreateOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileDefaultMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileDefaultMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Create a Message VPN Access-Level Exception object. Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> CreateOauthProfileDefaultMsgVpnAccessLevelExceptionAsync (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> localVarResponse = await CreateOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Message VPN Access-Level Exception object. Create a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x|x|||| oauthProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>> CreateOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileDefaultMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileDefaultMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Create a Required Claim object. Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||x||| resourceServerRequiredClaimName|x|x|||| resourceServerRequiredClaimValue||x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResourceServerRequiredClaimResponse</returns>
        public OauthProfileResourceServerRequiredClaimResponse CreateOauthProfileResourceServerRequiredClaim (OauthProfileResourceServerRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResourceServerRequiredClaimResponse> localVarResponse = CreateOauthProfileResourceServerRequiredClaimWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Required Claim object. Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||x||| resourceServerRequiredClaimName|x|x|||| resourceServerRequiredClaimValue||x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResourceServerRequiredClaimResponse</returns>
        public ApiResponse< OauthProfileResourceServerRequiredClaimResponse > CreateOauthProfileResourceServerRequiredClaimWithHttpInfo (OauthProfileResourceServerRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileResourceServerRequiredClaim");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileResourceServerRequiredClaim");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileResourceServerRequiredClaim", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResourceServerRequiredClaimResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResourceServerRequiredClaimResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResourceServerRequiredClaimResponse)));
        }

        /// <summary>
        /// Create a Required Claim object. Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||x||| resourceServerRequiredClaimName|x|x|||| resourceServerRequiredClaimValue||x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResourceServerRequiredClaimResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileResourceServerRequiredClaimResponse> CreateOauthProfileResourceServerRequiredClaimAsync (OauthProfileResourceServerRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResourceServerRequiredClaimResponse> localVarResponse = await CreateOauthProfileResourceServerRequiredClaimAsyncWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Required Claim object. Create a Required Claim object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||x||| resourceServerRequiredClaimName|x|x|||| resourceServerRequiredClaimValue||x||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Required Claim object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResourceServerRequiredClaimResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileResourceServerRequiredClaimResponse>> CreateOauthProfileResourceServerRequiredClaimAsyncWithHttpInfo (OauthProfileResourceServerRequiredClaim body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->CreateOauthProfileResourceServerRequiredClaim");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->CreateOauthProfileResourceServerRequiredClaim");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateOauthProfileResourceServerRequiredClaim", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResourceServerRequiredClaimResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResourceServerRequiredClaimResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResourceServerRequiredClaimResponse)));
        }

        /// <summary>
        /// Delete an OAuth Profile object. Delete an OAuth Profile object. The deletion of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteOauthProfile (string oauthProfileName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteOauthProfileWithHttpInfo(oauthProfileName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete an OAuth Profile object. Delete an OAuth Profile object. The deletion of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteOauthProfileWithHttpInfo (string oauthProfileName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfile");

            var localVarPath = "./oauthProfiles/{oauthProfileName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete an OAuth Profile object. Delete an OAuth Profile object. The deletion of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileAsync (string oauthProfileName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteOauthProfileAsyncWithHttpInfo(oauthProfileName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete an OAuth Profile object. Delete an OAuth Profile object. The deletion of instances of this object are synchronized to HA mates via config-sync.  OAuth profiles specify how to securely authenticate to an OAuth provider.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileAsyncWithHttpInfo (string oauthProfileName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfile");

            var localVarPath = "./oauthProfiles/{oauthProfileName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Group Access Level object. Delete a Group Access Level object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteOauthProfileAccessLevelGroup (string oauthProfileName, string groupName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteOauthProfileAccessLevelGroupWithHttpInfo(oauthProfileName, groupName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Group Access Level object. Delete a Group Access Level object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteOauthProfileAccessLevelGroupWithHttpInfo (string oauthProfileName, string groupName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileAccessLevelGroup");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->DeleteOauthProfileAccessLevelGroup");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileAccessLevelGroup", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Group Access Level object. Delete a Group Access Level object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileAccessLevelGroupAsync (string oauthProfileName, string groupName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteOauthProfileAccessLevelGroupAsyncWithHttpInfo(oauthProfileName, groupName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Group Access Level object. Delete a Group Access Level object. The deletion of instances of this object are synchronized to HA mates via config-sync.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileAccessLevelGroupAsyncWithHttpInfo (string oauthProfileName, string groupName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileAccessLevelGroup");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->DeleteOauthProfileAccessLevelGroup");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileAccessLevelGroup", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Message VPN Access-Level Exception object. Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException (string oauthProfileName, string groupName, string msgVpnName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo(oauthProfileName, groupName, msgVpnName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Message VPN Access-Level Exception object. Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo (string oauthProfileName, string groupName, string msgVpnName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Message VPN Access-Level Exception object. Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsync (string oauthProfileName, string groupName, string msgVpnName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo(oauthProfileName, groupName, msgVpnName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Message VPN Access-Level Exception object. Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Message VPN access-level exceptions for members of this group.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo (string oauthProfileName, string groupName, string msgVpnName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileAccessLevelGroupMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete an Allowed Host Value object. Delete an Allowed Host Value object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteOauthProfileClientAllowedHost (string oauthProfileName, string allowedHost)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteOauthProfileClientAllowedHostWithHttpInfo(oauthProfileName, allowedHost);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete an Allowed Host Value object. Delete an Allowed Host Value object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteOauthProfileClientAllowedHostWithHttpInfo (string oauthProfileName, string allowedHost)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileClientAllowedHost");
            // verify the required parameter 'allowedHost' is set
            if (allowedHost == null)
                throw new ApiException(400, "Missing required parameter 'allowedHost' when calling OauthProfileApi->DeleteOauthProfileClientAllowedHost");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAllowedHosts/{allowedHost}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (allowedHost != null) localVarPathParams.Add("allowedHost", this.Configuration.ApiClient.ParameterToString(allowedHost)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileClientAllowedHost", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete an Allowed Host Value object. Delete an Allowed Host Value object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileClientAllowedHostAsync (string oauthProfileName, string allowedHost)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteOauthProfileClientAllowedHostAsyncWithHttpInfo(oauthProfileName, allowedHost);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete an Allowed Host Value object. Delete an Allowed Host Value object. The deletion of instances of this object are synchronized to HA mates via config-sync.  A valid hostname for this broker in OAuth redirects.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileClientAllowedHostAsyncWithHttpInfo (string oauthProfileName, string allowedHost)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileClientAllowedHost");
            // verify the required parameter 'allowedHost' is set
            if (allowedHost == null)
                throw new ApiException(400, "Missing required parameter 'allowedHost' when calling OauthProfileApi->DeleteOauthProfileClientAllowedHost");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAllowedHosts/{allowedHost}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (allowedHost != null) localVarPathParams.Add("allowedHost", this.Configuration.ApiClient.ParameterToString(allowedHost)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileClientAllowedHost", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete an Authorization Parameter object. Delete an Authorization Parameter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteOauthProfileClientAuthorizationParameter (string oauthProfileName, string authorizationParameterName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteOauthProfileClientAuthorizationParameterWithHttpInfo(oauthProfileName, authorizationParameterName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete an Authorization Parameter object. Delete an Authorization Parameter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteOauthProfileClientAuthorizationParameterWithHttpInfo (string oauthProfileName, string authorizationParameterName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'authorizationParameterName' is set
            if (authorizationParameterName == null)
                throw new ApiException(400, "Missing required parameter 'authorizationParameterName' when calling OauthProfileApi->DeleteOauthProfileClientAuthorizationParameter");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (authorizationParameterName != null) localVarPathParams.Add("authorizationParameterName", this.Configuration.ApiClient.ParameterToString(authorizationParameterName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileClientAuthorizationParameter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete an Authorization Parameter object. Delete an Authorization Parameter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileClientAuthorizationParameterAsync (string oauthProfileName, string authorizationParameterName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteOauthProfileClientAuthorizationParameterAsyncWithHttpInfo(oauthProfileName, authorizationParameterName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete an Authorization Parameter object. Delete an Authorization Parameter object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional parameters to be passed to the OAuth authorization endpoint.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileClientAuthorizationParameterAsyncWithHttpInfo (string oauthProfileName, string authorizationParameterName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'authorizationParameterName' is set
            if (authorizationParameterName == null)
                throw new ApiException(400, "Missing required parameter 'authorizationParameterName' when calling OauthProfileApi->DeleteOauthProfileClientAuthorizationParameter");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (authorizationParameterName != null) localVarPathParams.Add("authorizationParameterName", this.Configuration.ApiClient.ParameterToString(authorizationParameterName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileClientAuthorizationParameter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Required Claim object. Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteOauthProfileClientRequiredClaim (string oauthProfileName, string clientRequiredClaimName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteOauthProfileClientRequiredClaimWithHttpInfo(oauthProfileName, clientRequiredClaimName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Required Claim object. Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteOauthProfileClientRequiredClaimWithHttpInfo (string oauthProfileName, string clientRequiredClaimName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileClientRequiredClaim");
            // verify the required parameter 'clientRequiredClaimName' is set
            if (clientRequiredClaimName == null)
                throw new ApiException(400, "Missing required parameter 'clientRequiredClaimName' when calling OauthProfileApi->DeleteOauthProfileClientRequiredClaim");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (clientRequiredClaimName != null) localVarPathParams.Add("clientRequiredClaimName", this.Configuration.ApiClient.ParameterToString(clientRequiredClaimName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileClientRequiredClaim", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Required Claim object. Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileClientRequiredClaimAsync (string oauthProfileName, string clientRequiredClaimName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteOauthProfileClientRequiredClaimAsyncWithHttpInfo(oauthProfileName, clientRequiredClaimName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Required Claim object. Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the ID token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileClientRequiredClaimAsyncWithHttpInfo (string oauthProfileName, string clientRequiredClaimName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileClientRequiredClaim");
            // verify the required parameter 'clientRequiredClaimName' is set
            if (clientRequiredClaimName == null)
                throw new ApiException(400, "Missing required parameter 'clientRequiredClaimName' when calling OauthProfileApi->DeleteOauthProfileClientRequiredClaim");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (clientRequiredClaimName != null) localVarPathParams.Add("clientRequiredClaimName", this.Configuration.ApiClient.ParameterToString(clientRequiredClaimName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileClientRequiredClaim", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Message VPN Access-Level Exception object. Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteOauthProfileDefaultMsgVpnAccessLevelException (string oauthProfileName, string msgVpnName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo(oauthProfileName, msgVpnName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Message VPN Access-Level Exception object. Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo (string oauthProfileName, string msgVpnName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->DeleteOauthProfileDefaultMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileDefaultMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Message VPN Access-Level Exception object. Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileDefaultMsgVpnAccessLevelExceptionAsync (string oauthProfileName, string msgVpnName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo(oauthProfileName, msgVpnName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Message VPN Access-Level Exception object. Delete a Message VPN Access-Level Exception object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Default message VPN access-level exceptions.  A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo (string oauthProfileName, string msgVpnName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->DeleteOauthProfileDefaultMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileDefaultMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Required Claim object. Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteOauthProfileResourceServerRequiredClaim (string oauthProfileName, string resourceServerRequiredClaimName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteOauthProfileResourceServerRequiredClaimWithHttpInfo(oauthProfileName, resourceServerRequiredClaimName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Required Claim object. Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteOauthProfileResourceServerRequiredClaimWithHttpInfo (string oauthProfileName, string resourceServerRequiredClaimName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileResourceServerRequiredClaim");
            // verify the required parameter 'resourceServerRequiredClaimName' is set
            if (resourceServerRequiredClaimName == null)
                throw new ApiException(400, "Missing required parameter 'resourceServerRequiredClaimName' when calling OauthProfileApi->DeleteOauthProfileResourceServerRequiredClaim");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (resourceServerRequiredClaimName != null) localVarPathParams.Add("resourceServerRequiredClaimName", this.Configuration.ApiClient.ParameterToString(resourceServerRequiredClaimName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileResourceServerRequiredClaim", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Required Claim object. Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteOauthProfileResourceServerRequiredClaimAsync (string oauthProfileName, string resourceServerRequiredClaimName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteOauthProfileResourceServerRequiredClaimAsyncWithHttpInfo(oauthProfileName, resourceServerRequiredClaimName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Required Claim object. Delete a Required Claim object. The deletion of instances of this object are synchronized to HA mates via config-sync.  Additional claims to be verified in the access token.  A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteOauthProfileResourceServerRequiredClaimAsyncWithHttpInfo (string oauthProfileName, string resourceServerRequiredClaimName)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->DeleteOauthProfileResourceServerRequiredClaim");
            // verify the required parameter 'resourceServerRequiredClaimName' is set
            if (resourceServerRequiredClaimName == null)
                throw new ApiException(400, "Missing required parameter 'resourceServerRequiredClaimName' when calling OauthProfileApi->DeleteOauthProfileResourceServerRequiredClaim");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (resourceServerRequiredClaimName != null) localVarPathParams.Add("resourceServerRequiredClaimName", this.Configuration.ApiClient.ParameterToString(resourceServerRequiredClaimName)); // path parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.DELETE, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("DeleteOauthProfileResourceServerRequiredClaim", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Get an OAuth Profile object. Get an OAuth Profile object.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResponse</returns>
        public OauthProfileResponse GetOauthProfile (string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResponse> localVarResponse = GetOauthProfileWithHttpInfo(oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get an OAuth Profile object. Get an OAuth Profile object.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResponse</returns>
        public ApiResponse< OauthProfileResponse > GetOauthProfileWithHttpInfo (string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfile");

            var localVarPath = "./oauthProfiles/{oauthProfileName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResponse)));
        }

        /// <summary>
        /// Get an OAuth Profile object. Get an OAuth Profile object.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileResponse> GetOauthProfileAsync (string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResponse> localVarResponse = await GetOauthProfileAsyncWithHttpInfo(oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get an OAuth Profile object. Get an OAuth Profile object.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileResponse>> GetOauthProfileAsyncWithHttpInfo (string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfile");

            var localVarPath = "./oauthProfiles/{oauthProfileName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResponse)));
        }

        /// <summary>
        /// Get a Group Access Level object. Get a Group Access Level object.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupResponse</returns>
        public OauthProfileAccessLevelGroupResponse GetOauthProfileAccessLevelGroup (string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupResponse> localVarResponse = GetOauthProfileAccessLevelGroupWithHttpInfo(oauthProfileName, groupName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Group Access Level object. Get a Group Access Level object.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupResponse</returns>
        public ApiResponse< OauthProfileAccessLevelGroupResponse > GetOauthProfileAccessLevelGroupWithHttpInfo (string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroup");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroup");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileAccessLevelGroup", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupResponse)));
        }

        /// <summary>
        /// Get a Group Access Level object. Get a Group Access Level object.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileAccessLevelGroupResponse> GetOauthProfileAccessLevelGroupAsync (string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupResponse> localVarResponse = await GetOauthProfileAccessLevelGroupAsyncWithHttpInfo(oauthProfileName, groupName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Group Access Level object. Get a Group Access Level object.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupResponse>> GetOauthProfileAccessLevelGroupAsyncWithHttpInfo (string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroup");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroup");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileAccessLevelGroup", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupResponse)));
        }

        /// <summary>
        /// Get a Message VPN Access-Level Exception object. Get a Message VPN Access-Level Exception object.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        public OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException (string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> localVarResponse = GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo(oauthProfileName, groupName, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Message VPN Access-Level Exception object. Get a Message VPN Access-Level Exception object.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        public ApiResponse< OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse > GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo (string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Get a Message VPN Access-Level Exception object. Get a Message VPN Access-Level Exception object.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsync (string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> localVarResponse = await GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo(oauthProfileName, groupName, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Message VPN Access-Level Exception object. Get a Message VPN Access-Level Exception object.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>> GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo (string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileAccessLevelGroupMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects. Get a list of Message VPN Access-Level Exception objects.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse</returns>
        public OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions (string oauthProfileName, string groupName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse> localVarResponse = GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsWithHttpInfo(oauthProfileName, groupName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects. Get a list of Message VPN Access-Level Exception objects.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse</returns>
        public ApiResponse< OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse > GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsWithHttpInfo (string oauthProfileName, string groupName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse)));
        }

        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects. Get a list of Message VPN Access-Level Exception objects.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse> GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsAsync (string oauthProfileName, string groupName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse> localVarResponse = await GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsAsyncWithHttpInfo(oauthProfileName, groupName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects. Get a list of Message VPN Access-Level Exception objects.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse>> GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsAsyncWithHttpInfo (string oauthProfileName, string groupName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionsResponse)));
        }

        /// <summary>
        /// Get a list of Group Access Level objects. Get a list of Group Access Level objects.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupsResponse</returns>
        public OauthProfileAccessLevelGroupsResponse GetOauthProfileAccessLevelGroups (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupsResponse> localVarResponse = GetOauthProfileAccessLevelGroupsWithHttpInfo(oauthProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Group Access Level objects. Get a list of Group Access Level objects.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupsResponse</returns>
        public ApiResponse< OauthProfileAccessLevelGroupsResponse > GetOauthProfileAccessLevelGroupsWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroups");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileAccessLevelGroups", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupsResponse)));
        }

        /// <summary>
        /// Get a list of Group Access Level objects. Get a list of Group Access Level objects.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupsResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileAccessLevelGroupsResponse> GetOauthProfileAccessLevelGroupsAsync (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupsResponse> localVarResponse = await GetOauthProfileAccessLevelGroupsAsyncWithHttpInfo(oauthProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Group Access Level objects. Get a list of Group Access Level objects.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: groupName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupsResponse>> GetOauthProfileAccessLevelGroupsAsyncWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileAccessLevelGroups");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileAccessLevelGroups", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupsResponse)));
        }

        /// <summary>
        /// Get an Allowed Host Value object. Get an Allowed Host Value object.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAllowedHostResponse</returns>
        public OauthProfileClientAllowedHostResponse GetOauthProfileClientAllowedHost (string oauthProfileName, string allowedHost, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAllowedHostResponse> localVarResponse = GetOauthProfileClientAllowedHostWithHttpInfo(oauthProfileName, allowedHost, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get an Allowed Host Value object. Get an Allowed Host Value object.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAllowedHostResponse</returns>
        public ApiResponse< OauthProfileClientAllowedHostResponse > GetOauthProfileClientAllowedHostWithHttpInfo (string oauthProfileName, string allowedHost, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileClientAllowedHost");
            // verify the required parameter 'allowedHost' is set
            if (allowedHost == null)
                throw new ApiException(400, "Missing required parameter 'allowedHost' when calling OauthProfileApi->GetOauthProfileClientAllowedHost");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAllowedHosts/{allowedHost}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (allowedHost != null) localVarPathParams.Add("allowedHost", this.Configuration.ApiClient.ParameterToString(allowedHost)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileClientAllowedHost", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAllowedHostResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAllowedHostResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAllowedHostResponse)));
        }

        /// <summary>
        /// Get an Allowed Host Value object. Get an Allowed Host Value object.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAllowedHostResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileClientAllowedHostResponse> GetOauthProfileClientAllowedHostAsync (string oauthProfileName, string allowedHost, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAllowedHostResponse> localVarResponse = await GetOauthProfileClientAllowedHostAsyncWithHttpInfo(oauthProfileName, allowedHost, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get an Allowed Host Value object. Get an Allowed Host Value object.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="allowedHost">An allowed value for the Host header.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAllowedHostResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAllowedHostResponse>> GetOauthProfileClientAllowedHostAsyncWithHttpInfo (string oauthProfileName, string allowedHost, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileClientAllowedHost");
            // verify the required parameter 'allowedHost' is set
            if (allowedHost == null)
                throw new ApiException(400, "Missing required parameter 'allowedHost' when calling OauthProfileApi->GetOauthProfileClientAllowedHost");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAllowedHosts/{allowedHost}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (allowedHost != null) localVarPathParams.Add("allowedHost", this.Configuration.ApiClient.ParameterToString(allowedHost)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileClientAllowedHost", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAllowedHostResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAllowedHostResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAllowedHostResponse)));
        }

        /// <summary>
        /// Get a list of Allowed Host Value objects. Get a list of Allowed Host Value objects.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAllowedHostsResponse</returns>
        public OauthProfileClientAllowedHostsResponse GetOauthProfileClientAllowedHosts (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAllowedHostsResponse> localVarResponse = GetOauthProfileClientAllowedHostsWithHttpInfo(oauthProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Allowed Host Value objects. Get a list of Allowed Host Value objects.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAllowedHostsResponse</returns>
        public ApiResponse< OauthProfileClientAllowedHostsResponse > GetOauthProfileClientAllowedHostsWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileClientAllowedHosts");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAllowedHosts";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileClientAllowedHosts", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAllowedHostsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAllowedHostsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAllowedHostsResponse)));
        }

        /// <summary>
        /// Get a list of Allowed Host Value objects. Get a list of Allowed Host Value objects.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAllowedHostsResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileClientAllowedHostsResponse> GetOauthProfileClientAllowedHostsAsync (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAllowedHostsResponse> localVarResponse = await GetOauthProfileClientAllowedHostsAsyncWithHttpInfo(oauthProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Allowed Host Value objects. Get a list of Allowed Host Value objects.  A valid hostname for this broker in OAuth redirects.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: allowedHost|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAllowedHostsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAllowedHostsResponse>> GetOauthProfileClientAllowedHostsAsyncWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileClientAllowedHosts");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAllowedHosts";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileClientAllowedHosts", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAllowedHostsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAllowedHostsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAllowedHostsResponse)));
        }

        /// <summary>
        /// Get an Authorization Parameter object. Get an Authorization Parameter object.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAuthorizationParameterResponse</returns>
        public OauthProfileClientAuthorizationParameterResponse GetOauthProfileClientAuthorizationParameter (string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAuthorizationParameterResponse> localVarResponse = GetOauthProfileClientAuthorizationParameterWithHttpInfo(oauthProfileName, authorizationParameterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get an Authorization Parameter object. Get an Authorization Parameter object.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAuthorizationParameterResponse</returns>
        public ApiResponse< OauthProfileClientAuthorizationParameterResponse > GetOauthProfileClientAuthorizationParameterWithHttpInfo (string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'authorizationParameterName' is set
            if (authorizationParameterName == null)
                throw new ApiException(400, "Missing required parameter 'authorizationParameterName' when calling OauthProfileApi->GetOauthProfileClientAuthorizationParameter");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (authorizationParameterName != null) localVarPathParams.Add("authorizationParameterName", this.Configuration.ApiClient.ParameterToString(authorizationParameterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileClientAuthorizationParameter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAuthorizationParameterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAuthorizationParameterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAuthorizationParameterResponse)));
        }

        /// <summary>
        /// Get an Authorization Parameter object. Get an Authorization Parameter object.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAuthorizationParameterResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileClientAuthorizationParameterResponse> GetOauthProfileClientAuthorizationParameterAsync (string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAuthorizationParameterResponse> localVarResponse = await GetOauthProfileClientAuthorizationParameterAsyncWithHttpInfo(oauthProfileName, authorizationParameterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get an Authorization Parameter object. Get an Authorization Parameter object.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAuthorizationParameterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAuthorizationParameterResponse>> GetOauthProfileClientAuthorizationParameterAsyncWithHttpInfo (string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'authorizationParameterName' is set
            if (authorizationParameterName == null)
                throw new ApiException(400, "Missing required parameter 'authorizationParameterName' when calling OauthProfileApi->GetOauthProfileClientAuthorizationParameter");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (authorizationParameterName != null) localVarPathParams.Add("authorizationParameterName", this.Configuration.ApiClient.ParameterToString(authorizationParameterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileClientAuthorizationParameter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAuthorizationParameterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAuthorizationParameterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAuthorizationParameterResponse)));
        }

        /// <summary>
        /// Get a list of Authorization Parameter objects. Get a list of Authorization Parameter objects.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAuthorizationParametersResponse</returns>
        public OauthProfileClientAuthorizationParametersResponse GetOauthProfileClientAuthorizationParameters (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAuthorizationParametersResponse> localVarResponse = GetOauthProfileClientAuthorizationParametersWithHttpInfo(oauthProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Authorization Parameter objects. Get a list of Authorization Parameter objects.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAuthorizationParametersResponse</returns>
        public ApiResponse< OauthProfileClientAuthorizationParametersResponse > GetOauthProfileClientAuthorizationParametersWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileClientAuthorizationParameters");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAuthorizationParameters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileClientAuthorizationParameters", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAuthorizationParametersResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAuthorizationParametersResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAuthorizationParametersResponse)));
        }

        /// <summary>
        /// Get a list of Authorization Parameter objects. Get a list of Authorization Parameter objects.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAuthorizationParametersResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileClientAuthorizationParametersResponse> GetOauthProfileClientAuthorizationParametersAsync (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAuthorizationParametersResponse> localVarResponse = await GetOauthProfileClientAuthorizationParametersAsyncWithHttpInfo(oauthProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Authorization Parameter objects. Get a list of Authorization Parameter objects.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: authorizationParameterName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAuthorizationParametersResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAuthorizationParametersResponse>> GetOauthProfileClientAuthorizationParametersAsyncWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileClientAuthorizationParameters");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAuthorizationParameters";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileClientAuthorizationParameters", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAuthorizationParametersResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAuthorizationParametersResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAuthorizationParametersResponse)));
        }

        /// <summary>
        /// Get a Required Claim object. Get a Required Claim object.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientRequiredClaimResponse</returns>
        public OauthProfileClientRequiredClaimResponse GetOauthProfileClientRequiredClaim (string oauthProfileName, string clientRequiredClaimName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientRequiredClaimResponse> localVarResponse = GetOauthProfileClientRequiredClaimWithHttpInfo(oauthProfileName, clientRequiredClaimName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Required Claim object. Get a Required Claim object.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientRequiredClaimResponse</returns>
        public ApiResponse< OauthProfileClientRequiredClaimResponse > GetOauthProfileClientRequiredClaimWithHttpInfo (string oauthProfileName, string clientRequiredClaimName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileClientRequiredClaim");
            // verify the required parameter 'clientRequiredClaimName' is set
            if (clientRequiredClaimName == null)
                throw new ApiException(400, "Missing required parameter 'clientRequiredClaimName' when calling OauthProfileApi->GetOauthProfileClientRequiredClaim");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (clientRequiredClaimName != null) localVarPathParams.Add("clientRequiredClaimName", this.Configuration.ApiClient.ParameterToString(clientRequiredClaimName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileClientRequiredClaim", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientRequiredClaimResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientRequiredClaimResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientRequiredClaimResponse)));
        }

        /// <summary>
        /// Get a Required Claim object. Get a Required Claim object.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientRequiredClaimResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileClientRequiredClaimResponse> GetOauthProfileClientRequiredClaimAsync (string oauthProfileName, string clientRequiredClaimName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientRequiredClaimResponse> localVarResponse = await GetOauthProfileClientRequiredClaimAsyncWithHttpInfo(oauthProfileName, clientRequiredClaimName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Required Claim object. Get a Required Claim object.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="clientRequiredClaimName">The name of the ID token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientRequiredClaimResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileClientRequiredClaimResponse>> GetOauthProfileClientRequiredClaimAsyncWithHttpInfo (string oauthProfileName, string clientRequiredClaimName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileClientRequiredClaim");
            // verify the required parameter 'clientRequiredClaimName' is set
            if (clientRequiredClaimName == null)
                throw new ApiException(400, "Missing required parameter 'clientRequiredClaimName' when calling OauthProfileApi->GetOauthProfileClientRequiredClaim");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientRequiredClaims/{clientRequiredClaimName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (clientRequiredClaimName != null) localVarPathParams.Add("clientRequiredClaimName", this.Configuration.ApiClient.ParameterToString(clientRequiredClaimName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileClientRequiredClaim", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientRequiredClaimResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientRequiredClaimResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientRequiredClaimResponse)));
        }

        /// <summary>
        /// Get a list of Required Claim objects. Get a list of Required Claim objects.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientRequiredClaimsResponse</returns>
        public OauthProfileClientRequiredClaimsResponse GetOauthProfileClientRequiredClaims (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientRequiredClaimsResponse> localVarResponse = GetOauthProfileClientRequiredClaimsWithHttpInfo(oauthProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Required Claim objects. Get a list of Required Claim objects.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientRequiredClaimsResponse</returns>
        public ApiResponse< OauthProfileClientRequiredClaimsResponse > GetOauthProfileClientRequiredClaimsWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileClientRequiredClaims");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientRequiredClaims";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileClientRequiredClaims", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientRequiredClaimsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientRequiredClaimsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientRequiredClaimsResponse)));
        }

        /// <summary>
        /// Get a list of Required Claim objects. Get a list of Required Claim objects.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientRequiredClaimsResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileClientRequiredClaimsResponse> GetOauthProfileClientRequiredClaimsAsync (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientRequiredClaimsResponse> localVarResponse = await GetOauthProfileClientRequiredClaimsAsyncWithHttpInfo(oauthProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Required Claim objects. Get a list of Required Claim objects.  Additional claims to be verified in the ID token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientRequiredClaimName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientRequiredClaimsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileClientRequiredClaimsResponse>> GetOauthProfileClientRequiredClaimsAsyncWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileClientRequiredClaims");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientRequiredClaims";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileClientRequiredClaims", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientRequiredClaimsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientRequiredClaimsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientRequiredClaimsResponse)));
        }

        /// <summary>
        /// Get a Message VPN Access-Level Exception object. Get a Message VPN Access-Level Exception object.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        public OauthProfileDefaultMsgVpnAccessLevelExceptionResponse GetOauthProfileDefaultMsgVpnAccessLevelException (string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> localVarResponse = GetOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo(oauthProfileName, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Message VPN Access-Level Exception object. Get a Message VPN Access-Level Exception object.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        public ApiResponse< OauthProfileDefaultMsgVpnAccessLevelExceptionResponse > GetOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo (string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->GetOauthProfileDefaultMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileDefaultMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Get a Message VPN Access-Level Exception object. Get a Message VPN Access-Level Exception object.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> GetOauthProfileDefaultMsgVpnAccessLevelExceptionAsync (string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> localVarResponse = await GetOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo(oauthProfileName, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Message VPN Access-Level Exception object. Get a Message VPN Access-Level Exception object.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>> GetOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo (string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->GetOauthProfileDefaultMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileDefaultMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects. Get a list of Message VPN Access-Level Exception objects.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse</returns>
        public OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse GetOauthProfileDefaultMsgVpnAccessLevelExceptions (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse> localVarResponse = GetOauthProfileDefaultMsgVpnAccessLevelExceptionsWithHttpInfo(oauthProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects. Get a list of Message VPN Access-Level Exception objects.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse</returns>
        public ApiResponse< OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse > GetOauthProfileDefaultMsgVpnAccessLevelExceptionsWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileDefaultMsgVpnAccessLevelExceptions");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileDefaultMsgVpnAccessLevelExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse)));
        }

        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects. Get a list of Message VPN Access-Level Exception objects.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse> GetOauthProfileDefaultMsgVpnAccessLevelExceptionsAsync (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse> localVarResponse = await GetOauthProfileDefaultMsgVpnAccessLevelExceptionsAsyncWithHttpInfo(oauthProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Message VPN Access-Level Exception objects. Get a list of Message VPN Access-Level Exception objects.  Default message VPN access-level exceptions.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse>> GetOauthProfileDefaultMsgVpnAccessLevelExceptionsAsyncWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileDefaultMsgVpnAccessLevelExceptions");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileDefaultMsgVpnAccessLevelExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileDefaultMsgVpnAccessLevelExceptionsResponse)));
        }

        /// <summary>
        /// Get a Required Claim object. Get a Required Claim object.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResourceServerRequiredClaimResponse</returns>
        public OauthProfileResourceServerRequiredClaimResponse GetOauthProfileResourceServerRequiredClaim (string oauthProfileName, string resourceServerRequiredClaimName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResourceServerRequiredClaimResponse> localVarResponse = GetOauthProfileResourceServerRequiredClaimWithHttpInfo(oauthProfileName, resourceServerRequiredClaimName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Required Claim object. Get a Required Claim object.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResourceServerRequiredClaimResponse</returns>
        public ApiResponse< OauthProfileResourceServerRequiredClaimResponse > GetOauthProfileResourceServerRequiredClaimWithHttpInfo (string oauthProfileName, string resourceServerRequiredClaimName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileResourceServerRequiredClaim");
            // verify the required parameter 'resourceServerRequiredClaimName' is set
            if (resourceServerRequiredClaimName == null)
                throw new ApiException(400, "Missing required parameter 'resourceServerRequiredClaimName' when calling OauthProfileApi->GetOauthProfileResourceServerRequiredClaim");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (resourceServerRequiredClaimName != null) localVarPathParams.Add("resourceServerRequiredClaimName", this.Configuration.ApiClient.ParameterToString(resourceServerRequiredClaimName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileResourceServerRequiredClaim", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResourceServerRequiredClaimResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResourceServerRequiredClaimResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResourceServerRequiredClaimResponse)));
        }

        /// <summary>
        /// Get a Required Claim object. Get a Required Claim object.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResourceServerRequiredClaimResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileResourceServerRequiredClaimResponse> GetOauthProfileResourceServerRequiredClaimAsync (string oauthProfileName, string resourceServerRequiredClaimName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResourceServerRequiredClaimResponse> localVarResponse = await GetOauthProfileResourceServerRequiredClaimAsyncWithHttpInfo(oauthProfileName, resourceServerRequiredClaimName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Required Claim object. Get a Required Claim object.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="resourceServerRequiredClaimName">The name of the access token claim to verify.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResourceServerRequiredClaimResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileResourceServerRequiredClaimResponse>> GetOauthProfileResourceServerRequiredClaimAsyncWithHttpInfo (string oauthProfileName, string resourceServerRequiredClaimName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileResourceServerRequiredClaim");
            // verify the required parameter 'resourceServerRequiredClaimName' is set
            if (resourceServerRequiredClaimName == null)
                throw new ApiException(400, "Missing required parameter 'resourceServerRequiredClaimName' when calling OauthProfileApi->GetOauthProfileResourceServerRequiredClaim");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims/{resourceServerRequiredClaimName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (resourceServerRequiredClaimName != null) localVarPathParams.Add("resourceServerRequiredClaimName", this.Configuration.ApiClient.ParameterToString(resourceServerRequiredClaimName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileResourceServerRequiredClaim", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResourceServerRequiredClaimResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResourceServerRequiredClaimResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResourceServerRequiredClaimResponse)));
        }

        /// <summary>
        /// Get a list of Required Claim objects. Get a list of Required Claim objects.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResourceServerRequiredClaimsResponse</returns>
        public OauthProfileResourceServerRequiredClaimsResponse GetOauthProfileResourceServerRequiredClaims (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResourceServerRequiredClaimsResponse> localVarResponse = GetOauthProfileResourceServerRequiredClaimsWithHttpInfo(oauthProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Required Claim objects. Get a list of Required Claim objects.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResourceServerRequiredClaimsResponse</returns>
        public ApiResponse< OauthProfileResourceServerRequiredClaimsResponse > GetOauthProfileResourceServerRequiredClaimsWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileResourceServerRequiredClaims");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileResourceServerRequiredClaims", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResourceServerRequiredClaimsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResourceServerRequiredClaimsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResourceServerRequiredClaimsResponse)));
        }

        /// <summary>
        /// Get a list of Required Claim objects. Get a list of Required Claim objects.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResourceServerRequiredClaimsResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileResourceServerRequiredClaimsResponse> GetOauthProfileResourceServerRequiredClaimsAsync (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResourceServerRequiredClaimsResponse> localVarResponse = await GetOauthProfileResourceServerRequiredClaimsAsyncWithHttpInfo(oauthProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Required Claim objects. Get a list of Required Claim objects.  Additional claims to be verified in the access token.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: oauthProfileName|x||| resourceServerRequiredClaimName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResourceServerRequiredClaimsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileResourceServerRequiredClaimsResponse>> GetOauthProfileResourceServerRequiredClaimsAsyncWithHttpInfo (string oauthProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->GetOauthProfileResourceServerRequiredClaims");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/resourceServerRequiredClaims";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfileResourceServerRequiredClaims", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResourceServerRequiredClaimsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResourceServerRequiredClaimsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResourceServerRequiredClaimsResponse)));
        }

        /// <summary>
        /// Get a list of OAuth Profile objects. Get a list of OAuth Profile objects.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfilesResponse</returns>
        public OauthProfilesResponse GetOauthProfiles (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfilesResponse> localVarResponse = GetOauthProfilesWithHttpInfo(count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of OAuth Profile objects. Get a list of OAuth Profile objects.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfilesResponse</returns>
        public ApiResponse< OauthProfilesResponse > GetOauthProfilesWithHttpInfo (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {

            var localVarPath = "./oauthProfiles";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfiles", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfilesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfilesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfilesResponse)));
        }

        /// <summary>
        /// Get a list of OAuth Profile objects. Get a list of OAuth Profile objects.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfilesResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfilesResponse> GetOauthProfilesAsync (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<OauthProfilesResponse> localVarResponse = await GetOauthProfilesAsyncWithHttpInfo(count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of OAuth Profile objects. Get a list of OAuth Profile objects.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: clientSecret||x||x oauthProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-only\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfilesResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfilesResponse>> GetOauthProfilesAsyncWithHttpInfo (int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {

            var localVarPath = "./oauthProfiles";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (count != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "count", count)); // query parameter
            if (cursor != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "cursor", cursor)); // query parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (where != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "where", where)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.GET, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("GetOauthProfiles", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfilesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfilesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfilesResponse)));
        }

        /// <summary>
        /// Replace an OAuth Profile object. Replace an OAuth Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret||||x||||x oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResponse</returns>
        public OauthProfileResponse ReplaceOauthProfile (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResponse> localVarResponse = ReplaceOauthProfileWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace an OAuth Profile object. Replace an OAuth Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret||||x||||x oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResponse</returns>
        public ApiResponse< OauthProfileResponse > ReplaceOauthProfileWithHttpInfo (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->ReplaceOauthProfile");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->ReplaceOauthProfile");

            var localVarPath = "./oauthProfiles/{oauthProfileName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceOauthProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResponse)));
        }

        /// <summary>
        /// Replace an OAuth Profile object. Replace an OAuth Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret||||x||||x oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileResponse> ReplaceOauthProfileAsync (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResponse> localVarResponse = await ReplaceOauthProfileAsyncWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace an OAuth Profile object. Replace an OAuth Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret||||x||||x oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileResponse>> ReplaceOauthProfileAsyncWithHttpInfo (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->ReplaceOauthProfile");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->ReplaceOauthProfile");

            var localVarPath = "./oauthProfiles/{oauthProfileName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceOauthProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResponse)));
        }

        /// <summary>
        /// Replace a Group Access Level object. Replace a Group Access Level object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupResponse</returns>
        public OauthProfileAccessLevelGroupResponse ReplaceOauthProfileAccessLevelGroup (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupResponse> localVarResponse = ReplaceOauthProfileAccessLevelGroupWithHttpInfo(body, oauthProfileName, groupName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Group Access Level object. Replace a Group Access Level object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupResponse</returns>
        public ApiResponse< OauthProfileAccessLevelGroupResponse > ReplaceOauthProfileAccessLevelGroupWithHttpInfo (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroup");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroup");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroup");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceOauthProfileAccessLevelGroup", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupResponse)));
        }

        /// <summary>
        /// Replace a Group Access Level object. Replace a Group Access Level object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileAccessLevelGroupResponse> ReplaceOauthProfileAccessLevelGroupAsync (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupResponse> localVarResponse = await ReplaceOauthProfileAccessLevelGroupAsyncWithHttpInfo(body, oauthProfileName, groupName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Group Access Level object. Replace a Group Access Level object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupResponse>> ReplaceOauthProfileAccessLevelGroupAsyncWithHttpInfo (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroup");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroup");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroup");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceOauthProfileAccessLevelGroup", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupResponse)));
        }

        /// <summary>
        /// Replace a Message VPN Access-Level Exception object. Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        public OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> localVarResponse = ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo(body, oauthProfileName, groupName, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Message VPN Access-Level Exception object. Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        public ApiResponse< OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse > ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Replace a Message VPN Access-Level Exception object. Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsync (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> localVarResponse = await ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo(body, oauthProfileName, groupName, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Message VPN Access-Level Exception object. Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x||x||||| msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>> ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceOauthProfileAccessLevelGroupMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Replace an Authorization Parameter object. Replace an Authorization Parameter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAuthorizationParameterResponse</returns>
        public OauthProfileClientAuthorizationParameterResponse ReplaceOauthProfileClientAuthorizationParameter (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAuthorizationParameterResponse> localVarResponse = ReplaceOauthProfileClientAuthorizationParameterWithHttpInfo(body, oauthProfileName, authorizationParameterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace an Authorization Parameter object. Replace an Authorization Parameter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAuthorizationParameterResponse</returns>
        public ApiResponse< OauthProfileClientAuthorizationParameterResponse > ReplaceOauthProfileClientAuthorizationParameterWithHttpInfo (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->ReplaceOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->ReplaceOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'authorizationParameterName' is set
            if (authorizationParameterName == null)
                throw new ApiException(400, "Missing required parameter 'authorizationParameterName' when calling OauthProfileApi->ReplaceOauthProfileClientAuthorizationParameter");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (authorizationParameterName != null) localVarPathParams.Add("authorizationParameterName", this.Configuration.ApiClient.ParameterToString(authorizationParameterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceOauthProfileClientAuthorizationParameter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAuthorizationParameterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAuthorizationParameterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAuthorizationParameterResponse)));
        }

        /// <summary>
        /// Replace an Authorization Parameter object. Replace an Authorization Parameter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAuthorizationParameterResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileClientAuthorizationParameterResponse> ReplaceOauthProfileClientAuthorizationParameterAsync (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAuthorizationParameterResponse> localVarResponse = await ReplaceOauthProfileClientAuthorizationParameterAsyncWithHttpInfo(body, oauthProfileName, authorizationParameterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace an Authorization Parameter object. Replace an Authorization Parameter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAuthorizationParameterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAuthorizationParameterResponse>> ReplaceOauthProfileClientAuthorizationParameterAsyncWithHttpInfo (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->ReplaceOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->ReplaceOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'authorizationParameterName' is set
            if (authorizationParameterName == null)
                throw new ApiException(400, "Missing required parameter 'authorizationParameterName' when calling OauthProfileApi->ReplaceOauthProfileClientAuthorizationParameter");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (authorizationParameterName != null) localVarPathParams.Add("authorizationParameterName", this.Configuration.ApiClient.ParameterToString(authorizationParameterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceOauthProfileClientAuthorizationParameter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAuthorizationParameterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAuthorizationParameterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAuthorizationParameterResponse)));
        }

        /// <summary>
        /// Replace a Message VPN Access-Level Exception object. Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Default message VPN access-level exceptions.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        public OauthProfileDefaultMsgVpnAccessLevelExceptionResponse ReplaceOauthProfileDefaultMsgVpnAccessLevelException (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> localVarResponse = ReplaceOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo(body, oauthProfileName, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Message VPN Access-Level Exception object. Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Default message VPN access-level exceptions.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        public ApiResponse< OauthProfileDefaultMsgVpnAccessLevelExceptionResponse > ReplaceOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->ReplaceOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->ReplaceOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->ReplaceOauthProfileDefaultMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceOauthProfileDefaultMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Replace a Message VPN Access-Level Exception object. Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Default message VPN access-level exceptions.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> ReplaceOauthProfileDefaultMsgVpnAccessLevelExceptionAsync (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> localVarResponse = await ReplaceOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo(body, oauthProfileName, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Message VPN Access-Level Exception object. Replace a Message VPN Access-Level Exception object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Default message VPN access-level exceptions.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| oauthProfileName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>> ReplaceOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->ReplaceOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->ReplaceOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->ReplaceOauthProfileDefaultMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PUT, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("ReplaceOauthProfileDefaultMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Update an OAuth Profile object. Update an OAuth Profile object. Any attribute missing from the request will be left unchanged.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret|||x||||x oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileResponse</returns>
        public OauthProfileResponse UpdateOauthProfile (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResponse> localVarResponse = UpdateOauthProfileWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update an OAuth Profile object. Update an OAuth Profile object. Any attribute missing from the request will be left unchanged.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret|||x||||x oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileResponse</returns>
        public ApiResponse< OauthProfileResponse > UpdateOauthProfileWithHttpInfo (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->UpdateOauthProfile");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->UpdateOauthProfile");

            var localVarPath = "./oauthProfiles/{oauthProfileName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateOauthProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResponse)));
        }

        /// <summary>
        /// Update an OAuth Profile object. Update an OAuth Profile object. Any attribute missing from the request will be left unchanged.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret|||x||||x oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileResponse> UpdateOauthProfileAsync (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileResponse> localVarResponse = await UpdateOauthProfileAsyncWithHttpInfo(body, oauthProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update an OAuth Profile object. Update an OAuth Profile object. Any attribute missing from the request will be left unchanged.  OAuth profiles specify how to securely authenticate to an OAuth provider.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- clientSecret|||x||||x oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: accessLevelGroupsClaimName|global/admin accessLevelGroupsClaimStringFormat|global/admin clientId|global/admin clientRedirectUri|global/admin clientRequiredType|global/admin clientScope|global/admin clientSecret|global/admin clientValidateTypeEnabled|global/admin defaultGlobalAccessLevel|global/admin displayName|global/admin enabled|global/admin endpointAuthorization|global/admin endpointDiscovery|global/admin endpointDiscoveryRefreshInterval|global/admin endpointIntrospection|global/admin endpointIntrospectionTimeout|global/admin endpointJwks|global/admin endpointJwksRefreshInterval|global/admin endpointToken|global/admin endpointTokenTimeout|global/admin endpointUserinfo|global/admin endpointUserinfoTimeout|global/admin interactiveEnabled|global/admin interactivePromptForExpiredSession|global/admin interactivePromptForNewSession|global/admin issuer|global/admin oauthRole|global/admin resourceServerParseAccessTokenEnabled|global/admin resourceServerRequiredAudience|global/admin resourceServerRequiredIssuer|global/admin resourceServerRequiredScope|global/admin resourceServerRequiredType|global/admin resourceServerValidateAudienceEnabled|global/admin resourceServerValidateIssuerEnabled|global/admin resourceServerValidateScopeEnabled|global/admin resourceServerValidateTypeEnabled|global/admin sempEnabled|global/admin usernameClaimName|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The OAuth Profile object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileResponse>> UpdateOauthProfileAsyncWithHttpInfo (OauthProfile body, string oauthProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->UpdateOauthProfile");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->UpdateOauthProfile");

            var localVarPath = "./oauthProfiles/{oauthProfileName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateOauthProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileResponse)));
        }

        /// <summary>
        /// Update a Group Access Level object. Update a Group Access Level object. Any attribute missing from the request will be left unchanged.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupResponse</returns>
        public OauthProfileAccessLevelGroupResponse UpdateOauthProfileAccessLevelGroup (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupResponse> localVarResponse = UpdateOauthProfileAccessLevelGroupWithHttpInfo(body, oauthProfileName, groupName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Group Access Level object. Update a Group Access Level object. Any attribute missing from the request will be left unchanged.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupResponse</returns>
        public ApiResponse< OauthProfileAccessLevelGroupResponse > UpdateOauthProfileAccessLevelGroupWithHttpInfo (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroup");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroup");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroup");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateOauthProfileAccessLevelGroup", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupResponse)));
        }

        /// <summary>
        /// Update a Group Access Level object. Update a Group Access Level object. Any attribute missing from the request will be left unchanged.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileAccessLevelGroupResponse> UpdateOauthProfileAccessLevelGroupAsync (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupResponse> localVarResponse = await UpdateOauthProfileAccessLevelGroupAsyncWithHttpInfo(body, oauthProfileName, groupName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Group Access Level object. Update a Group Access Level object. Any attribute missing from the request will be left unchanged.  The name of a group as it exists on the OAuth server being used to authenticate SEMP users.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation. Requests which include the following attributes require greater access scope/level:   Attribute|Access Scope/Level :- --|:- --: globalAccessLevel|global/admin    This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Group Access Level object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupResponse>> UpdateOauthProfileAccessLevelGroupAsyncWithHttpInfo (OauthProfileAccessLevelGroup body, string oauthProfileName, string groupName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroup");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroup");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroup");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateOauthProfileAccessLevelGroup", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupResponse)));
        }

        /// <summary>
        /// Update a Message VPN Access-Level Exception object. Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        public OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> localVarResponse = UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo(body, oauthProfileName, groupName, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Message VPN Access-Level Exception object. Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        public ApiResponse< OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse > UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionWithHttpInfo (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Update a Message VPN Access-Level Exception object. Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsync (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse> localVarResponse = await UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo(body, oauthProfileName, groupName, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Message VPN Access-Level Exception object. Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Message VPN access-level exceptions for members of this group.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- groupName|x|x||||| msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="groupName">The name of the group.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>> UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionAsyncWithHttpInfo (OauthProfileAccessLevelGroupMsgVpnAccessLevelException body, string oauthProfileName, string groupName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'groupName' is set
            if (groupName == null)
                throw new ApiException(400, "Missing required parameter 'groupName' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/accessLevelGroups/{groupName}/msgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (groupName != null) localVarPathParams.Add("groupName", this.Configuration.ApiClient.ParameterToString(groupName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateOauthProfileAccessLevelGroupMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileAccessLevelGroupMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Update an Authorization Parameter object. Update an Authorization Parameter object. Any attribute missing from the request will be left unchanged.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileClientAuthorizationParameterResponse</returns>
        public OauthProfileClientAuthorizationParameterResponse UpdateOauthProfileClientAuthorizationParameter (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAuthorizationParameterResponse> localVarResponse = UpdateOauthProfileClientAuthorizationParameterWithHttpInfo(body, oauthProfileName, authorizationParameterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update an Authorization Parameter object. Update an Authorization Parameter object. Any attribute missing from the request will be left unchanged.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileClientAuthorizationParameterResponse</returns>
        public ApiResponse< OauthProfileClientAuthorizationParameterResponse > UpdateOauthProfileClientAuthorizationParameterWithHttpInfo (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->UpdateOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->UpdateOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'authorizationParameterName' is set
            if (authorizationParameterName == null)
                throw new ApiException(400, "Missing required parameter 'authorizationParameterName' when calling OauthProfileApi->UpdateOauthProfileClientAuthorizationParameter");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (authorizationParameterName != null) localVarPathParams.Add("authorizationParameterName", this.Configuration.ApiClient.ParameterToString(authorizationParameterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateOauthProfileClientAuthorizationParameter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAuthorizationParameterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAuthorizationParameterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAuthorizationParameterResponse)));
        }

        /// <summary>
        /// Update an Authorization Parameter object. Update an Authorization Parameter object. Any attribute missing from the request will be left unchanged.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileClientAuthorizationParameterResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileClientAuthorizationParameterResponse> UpdateOauthProfileClientAuthorizationParameterAsync (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileClientAuthorizationParameterResponse> localVarResponse = await UpdateOauthProfileClientAuthorizationParameterAsyncWithHttpInfo(body, oauthProfileName, authorizationParameterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update an Authorization Parameter object. Update an Authorization Parameter object. Any attribute missing from the request will be left unchanged.  Additional parameters to be passed to the OAuth authorization endpoint.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- authorizationParameterName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/admin\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Authorization Parameter object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="authorizationParameterName">The name of the authorization parameter.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileClientAuthorizationParameterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileClientAuthorizationParameterResponse>> UpdateOauthProfileClientAuthorizationParameterAsyncWithHttpInfo (OauthProfileClientAuthorizationParameter body, string oauthProfileName, string authorizationParameterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->UpdateOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->UpdateOauthProfileClientAuthorizationParameter");
            // verify the required parameter 'authorizationParameterName' is set
            if (authorizationParameterName == null)
                throw new ApiException(400, "Missing required parameter 'authorizationParameterName' when calling OauthProfileApi->UpdateOauthProfileClientAuthorizationParameter");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/clientAuthorizationParameters/{authorizationParameterName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (authorizationParameterName != null) localVarPathParams.Add("authorizationParameterName", this.Configuration.ApiClient.ParameterToString(authorizationParameterName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateOauthProfileClientAuthorizationParameter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileClientAuthorizationParameterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileClientAuthorizationParameterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileClientAuthorizationParameterResponse)));
        }

        /// <summary>
        /// Update a Message VPN Access-Level Exception object. Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Default message VPN access-level exceptions.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        public OauthProfileDefaultMsgVpnAccessLevelExceptionResponse UpdateOauthProfileDefaultMsgVpnAccessLevelException (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> localVarResponse = UpdateOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo(body, oauthProfileName, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Message VPN Access-Level Exception object. Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Default message VPN access-level exceptions.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        public ApiResponse< OauthProfileDefaultMsgVpnAccessLevelExceptionResponse > UpdateOauthProfileDefaultMsgVpnAccessLevelExceptionWithHttpInfo (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->UpdateOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->UpdateOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->UpdateOauthProfileDefaultMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) this.Configuration.ApiClient.CallApi(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateOauthProfileDefaultMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)));
        }

        /// <summary>
        /// Update a Message VPN Access-Level Exception object. Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Default message VPN access-level exceptions.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of OauthProfileDefaultMsgVpnAccessLevelExceptionResponse</returns>
        public async System.Threading.Tasks.Task<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> UpdateOauthProfileDefaultMsgVpnAccessLevelExceptionAsync (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse> localVarResponse = await UpdateOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo(body, oauthProfileName, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Message VPN Access-Level Exception object. Update a Message VPN Access-Level Exception object. Any attribute missing from the request will be left unchanged.  Default message VPN access-level exceptions.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| oauthProfileName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;global/read-write\&quot; is required to perform this operation.  This has been available since 2.24.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Message VPN Access-Level Exception object&#x27;s attributes.</param>
        /// <param name="oauthProfileName">The name of the OAuth profile.</param>
        /// <param name="msgVpnName">The name of the message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>> UpdateOauthProfileDefaultMsgVpnAccessLevelExceptionAsyncWithHttpInfo (OauthProfileDefaultMsgVpnAccessLevelException body, string oauthProfileName, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling OauthProfileApi->UpdateOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'oauthProfileName' is set
            if (oauthProfileName == null)
                throw new ApiException(400, "Missing required parameter 'oauthProfileName' when calling OauthProfileApi->UpdateOauthProfileDefaultMsgVpnAccessLevelException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling OauthProfileApi->UpdateOauthProfileDefaultMsgVpnAccessLevelException");

            var localVarPath = "./oauthProfiles/{oauthProfileName}/defaultMsgVpnAccessLevelExceptions/{msgVpnName}";
            var localVarPathParams = new Dictionary<String, String>();
            var localVarQueryParams = new List<KeyValuePair<String, String>>();
            var localVarHeaderParams = new Dictionary<String, String>(this.Configuration.DefaultHeader);
            var localVarFormParams = new Dictionary<String, String>();
            var localVarFileParams = new Dictionary<String, FileParameter>();
            Object localVarPostBody = null;

            // to determine the Content-Type header
            String[] localVarHttpContentTypes = new String[] {
                "application/json"
            };
            String localVarHttpContentType = this.Configuration.ApiClient.SelectHeaderContentType(localVarHttpContentTypes);

            // to determine the Accept header
            String[] localVarHttpHeaderAccepts = new String[] {
                "application/json"
            };
            String localVarHttpHeaderAccept = this.Configuration.ApiClient.SelectHeaderAccept(localVarHttpHeaderAccepts);
            if (localVarHttpHeaderAccept != null)
                localVarHeaderParams.Add("Accept", localVarHttpHeaderAccept);

            if (oauthProfileName != null) localVarPathParams.Add("oauthProfileName", this.Configuration.ApiClient.ParameterToString(oauthProfileName)); // path parameter
            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (opaquePassword != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("", "opaquePassword", opaquePassword)); // query parameter
            if (select != null) localVarQueryParams.AddRange(this.Configuration.ApiClient.ParameterToKeyValuePairs("csv", "select", select)); // query parameter
            if (body != null && body.GetType() != typeof(byte[]))
            {
                localVarPostBody = this.Configuration.ApiClient.Serialize(body); // http body (model) parameter
            }
            else
            {
                localVarPostBody = body; // byte array
            }
            // authentication (basicAuth) required
            // http basic authentication required
            if (!String.IsNullOrEmpty(this.Configuration.Username) || !String.IsNullOrEmpty(this.Configuration.Password))
            {
                localVarHeaderParams["Authorization"] = "Basic " + ApiClient.Base64Encode(this.Configuration.Username + ":" + this.Configuration.Password);
            }

            // make the HTTP request
            IRestResponse localVarResponse = (IRestResponse) await this.Configuration.ApiClient.CallApiAsync(localVarPath,
                Method.PATCH, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("UpdateOauthProfileDefaultMsgVpnAccessLevelException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<OauthProfileDefaultMsgVpnAccessLevelExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (OauthProfileDefaultMsgVpnAccessLevelExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(OauthProfileDefaultMsgVpnAccessLevelExceptionResponse)));
        }

    }
}
