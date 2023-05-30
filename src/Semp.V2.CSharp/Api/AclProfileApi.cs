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
        public interface IAclProfileApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Create an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Create an ACL Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileResponse</returns>
        MsgVpnAclProfileResponse CreateMsgVpnAclProfile (MsgVpnAclProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Create an ACL Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileResponse</returns>
        ApiResponse<MsgVpnAclProfileResponse> CreateMsgVpnAclProfileWithHttpInfo (MsgVpnAclProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Client Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Client Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| clientConnectExceptionAddress|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Client Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileClientConnectExceptionResponse</returns>
        MsgVpnAclProfileClientConnectExceptionResponse CreateMsgVpnAclProfileClientConnectException (MsgVpnAclProfileClientConnectException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Client Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Client Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| clientConnectExceptionAddress|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Client Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileClientConnectExceptionResponse</returns>
        ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse> CreateMsgVpnAclProfileClientConnectExceptionWithHttpInfo (MsgVpnAclProfileClientConnectException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| publishExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilePublishExceptionResponse</returns>
        MsgVpnAclProfilePublishExceptionResponse CreateMsgVpnAclProfilePublishException (MsgVpnAclProfilePublishException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| publishExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilePublishExceptionResponse</returns>
        ApiResponse<MsgVpnAclProfilePublishExceptionResponse> CreateMsgVpnAclProfilePublishExceptionWithHttpInfo (MsgVpnAclProfilePublishException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| publishTopicException|x|x|||| publishTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilePublishTopicExceptionResponse</returns>
        MsgVpnAclProfilePublishTopicExceptionResponse CreateMsgVpnAclProfilePublishTopicException (MsgVpnAclProfilePublishTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| publishTopicException|x|x|||| publishTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilePublishTopicExceptionResponse</returns>
        ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse> CreateMsgVpnAclProfilePublishTopicExceptionWithHttpInfo (MsgVpnAclProfilePublishTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| subscribeExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeExceptionResponse</returns>
        MsgVpnAclProfileSubscribeExceptionResponse CreateMsgVpnAclProfileSubscribeException (MsgVpnAclProfileSubscribeException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| subscribeExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeExceptionResponse</returns>
        ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse> CreateMsgVpnAclProfileSubscribeExceptionWithHttpInfo (MsgVpnAclProfileSubscribeException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Subscribe Share Name Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Subscribe Share Name Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeShareNameException|x|x|||| subscribeShareNameExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Share Name Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeShareNameExceptionResponse</returns>
        MsgVpnAclProfileSubscribeShareNameExceptionResponse CreateMsgVpnAclProfileSubscribeShareNameException (MsgVpnAclProfileSubscribeShareNameException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Subscribe Share Name Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Subscribe Share Name Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeShareNameException|x|x|||| subscribeShareNameExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Share Name Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeShareNameExceptionResponse</returns>
        ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse> CreateMsgVpnAclProfileSubscribeShareNameExceptionWithHttpInfo (MsgVpnAclProfileSubscribeShareNameException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeTopicException|x|x|||| subscribeTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeTopicExceptionResponse</returns>
        MsgVpnAclProfileSubscribeTopicExceptionResponse CreateMsgVpnAclProfileSubscribeTopicException (MsgVpnAclProfileSubscribeTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeTopicException|x|x|||| subscribeTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeTopicExceptionResponse</returns>
        ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse> CreateMsgVpnAclProfileSubscribeTopicExceptionWithHttpInfo (MsgVpnAclProfileSubscribeTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Delete an ACL Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnAclProfile (string msgVpnName, string aclProfileName);

        /// <summary>
        /// Delete an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Delete an ACL Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnAclProfileWithHttpInfo (string msgVpnName, string aclProfileName);
        /// <summary>
        /// Delete a Client Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Client Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnAclProfileClientConnectException (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress);

        /// <summary>
        /// Delete a Client Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Client Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnAclProfileClientConnectExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress);
        /// <summary>
        /// Delete a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnAclProfilePublishException (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic);

        /// <summary>
        /// Delete a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnAclProfilePublishExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic);
        /// <summary>
        /// Delete a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnAclProfilePublishTopicException (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException);

        /// <summary>
        /// Delete a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnAclProfilePublishTopicExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException);
        /// <summary>
        /// Delete a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnAclProfileSubscribeException (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic);

        /// <summary>
        /// Delete a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnAclProfileSubscribeExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic);
        /// <summary>
        /// Delete a Subscribe Share Name Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Subscribe Share Name Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnAclProfileSubscribeShareNameException (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException);

        /// <summary>
        /// Delete a Subscribe Share Name Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Subscribe Share Name Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnAclProfileSubscribeShareNameExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException);
        /// <summary>
        /// Delete a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnAclProfileSubscribeTopicException (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException);

        /// <summary>
        /// Delete a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnAclProfileSubscribeTopicExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException);
        /// <summary>
        /// Get an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Get an ACL Profile object.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileResponse</returns>
        MsgVpnAclProfileResponse GetMsgVpnAclProfile (string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Get an ACL Profile object.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileResponse</returns>
        ApiResponse<MsgVpnAclProfileResponse> GetMsgVpnAclProfileWithHttpInfo (string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Client Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Client Connect Exception object.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileClientConnectExceptionResponse</returns>
        MsgVpnAclProfileClientConnectExceptionResponse GetMsgVpnAclProfileClientConnectException (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Client Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Client Connect Exception object.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileClientConnectExceptionResponse</returns>
        ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse> GetMsgVpnAclProfileClientConnectExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Client Connect Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Client Connect Exception objects.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileClientConnectExceptionsResponse</returns>
        MsgVpnAclProfileClientConnectExceptionsResponse GetMsgVpnAclProfileClientConnectExceptions (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Client Connect Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Client Connect Exception objects.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileClientConnectExceptionsResponse</returns>
        ApiResponse<MsgVpnAclProfileClientConnectExceptionsResponse> GetMsgVpnAclProfileClientConnectExceptionsWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilePublishExceptionResponse</returns>
        MsgVpnAclProfilePublishExceptionResponse GetMsgVpnAclProfilePublishException (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilePublishExceptionResponse</returns>
        ApiResponse<MsgVpnAclProfilePublishExceptionResponse> GetMsgVpnAclProfilePublishExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Publish Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilePublishExceptionsResponse</returns>
        MsgVpnAclProfilePublishExceptionsResponse GetMsgVpnAclProfilePublishExceptions (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Publish Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilePublishExceptionsResponse</returns>
        ApiResponse<MsgVpnAclProfilePublishExceptionsResponse> GetMsgVpnAclProfilePublishExceptionsWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilePublishTopicExceptionResponse</returns>
        MsgVpnAclProfilePublishTopicExceptionResponse GetMsgVpnAclProfilePublishTopicException (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilePublishTopicExceptionResponse</returns>
        ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse> GetMsgVpnAclProfilePublishTopicExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Publish Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilePublishTopicExceptionsResponse</returns>
        MsgVpnAclProfilePublishTopicExceptionsResponse GetMsgVpnAclProfilePublishTopicExceptions (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Publish Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilePublishTopicExceptionsResponse</returns>
        ApiResponse<MsgVpnAclProfilePublishTopicExceptionsResponse> GetMsgVpnAclProfilePublishTopicExceptionsWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeExceptionResponse</returns>
        MsgVpnAclProfileSubscribeExceptionResponse GetMsgVpnAclProfileSubscribeException (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeExceptionResponse</returns>
        ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse> GetMsgVpnAclProfileSubscribeExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Subscribe Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeExceptionsResponse</returns>
        MsgVpnAclProfileSubscribeExceptionsResponse GetMsgVpnAclProfileSubscribeExceptions (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Subscribe Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeExceptionsResponse</returns>
        ApiResponse<MsgVpnAclProfileSubscribeExceptionsResponse> GetMsgVpnAclProfileSubscribeExceptionsWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Subscribe Share Name Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Subscribe Share Name Exception object.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeShareNameExceptionResponse</returns>
        MsgVpnAclProfileSubscribeShareNameExceptionResponse GetMsgVpnAclProfileSubscribeShareNameException (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Subscribe Share Name Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Subscribe Share Name Exception object.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeShareNameExceptionResponse</returns>
        ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse> GetMsgVpnAclProfileSubscribeShareNameExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Subscribe Share Name Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Subscribe Share Name Exception objects.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeShareNameExceptionsResponse</returns>
        MsgVpnAclProfileSubscribeShareNameExceptionsResponse GetMsgVpnAclProfileSubscribeShareNameExceptions (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Subscribe Share Name Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Subscribe Share Name Exception objects.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeShareNameExceptionsResponse</returns>
        ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionsResponse> GetMsgVpnAclProfileSubscribeShareNameExceptionsWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeTopicExceptionResponse</returns>
        MsgVpnAclProfileSubscribeTopicExceptionResponse GetMsgVpnAclProfileSubscribeTopicException (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeTopicExceptionResponse</returns>
        ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse> GetMsgVpnAclProfileSubscribeTopicExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Subscribe Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeTopicExceptionsResponse</returns>
        MsgVpnAclProfileSubscribeTopicExceptionsResponse GetMsgVpnAclProfileSubscribeTopicExceptions (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Subscribe Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeTopicExceptionsResponse</returns>
        ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionsResponse> GetMsgVpnAclProfileSubscribeTopicExceptionsWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of ACL Profile objects.
        /// </summary>
        /// <remarks>
        /// Get a list of ACL Profile objects.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilesResponse</returns>
        MsgVpnAclProfilesResponse GetMsgVpnAclProfiles (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of ACL Profile objects.
        /// </summary>
        /// <remarks>
        /// Get a list of ACL Profile objects.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilesResponse</returns>
        ApiResponse<MsgVpnAclProfilesResponse> GetMsgVpnAclProfilesWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Replace an ACL Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileResponse</returns>
        MsgVpnAclProfileResponse ReplaceMsgVpnAclProfile (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Replace an ACL Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileResponse</returns>
        ApiResponse<MsgVpnAclProfileResponse> ReplaceMsgVpnAclProfileWithHttpInfo (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Update an ACL Profile object. Any attribute missing from the request will be left unchanged.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileResponse</returns>
        MsgVpnAclProfileResponse UpdateMsgVpnAclProfile (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Update an ACL Profile object. Any attribute missing from the request will be left unchanged.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileResponse</returns>
        ApiResponse<MsgVpnAclProfileResponse> UpdateMsgVpnAclProfileWithHttpInfo (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// Create an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Create an ACL Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileResponse> CreateMsgVpnAclProfileAsync (MsgVpnAclProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Create an ACL Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileResponse>> CreateMsgVpnAclProfileAsyncWithHttpInfo (MsgVpnAclProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Client Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Client Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| clientConnectExceptionAddress|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Client Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileClientConnectExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileClientConnectExceptionResponse> CreateMsgVpnAclProfileClientConnectExceptionAsync (MsgVpnAclProfileClientConnectException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Client Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Client Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| clientConnectExceptionAddress|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Client Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileClientConnectExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse>> CreateMsgVpnAclProfileClientConnectExceptionAsyncWithHttpInfo (MsgVpnAclProfileClientConnectException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| publishExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilePublishExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfilePublishExceptionResponse> CreateMsgVpnAclProfilePublishExceptionAsync (MsgVpnAclProfilePublishException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| publishExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilePublishExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilePublishExceptionResponse>> CreateMsgVpnAclProfilePublishExceptionAsyncWithHttpInfo (MsgVpnAclProfilePublishException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| publishTopicException|x|x|||| publishTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilePublishTopicExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfilePublishTopicExceptionResponse> CreateMsgVpnAclProfilePublishTopicExceptionAsync (MsgVpnAclProfilePublishTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| publishTopicException|x|x|||| publishTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilePublishTopicExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse>> CreateMsgVpnAclProfilePublishTopicExceptionAsyncWithHttpInfo (MsgVpnAclProfilePublishTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| subscribeExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeExceptionResponse> CreateMsgVpnAclProfileSubscribeExceptionAsync (MsgVpnAclProfileSubscribeException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| subscribeExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse>> CreateMsgVpnAclProfileSubscribeExceptionAsyncWithHttpInfo (MsgVpnAclProfileSubscribeException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Subscribe Share Name Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Subscribe Share Name Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeShareNameException|x|x|||| subscribeShareNameExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Share Name Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeShareNameExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeShareNameExceptionResponse> CreateMsgVpnAclProfileSubscribeShareNameExceptionAsync (MsgVpnAclProfileSubscribeShareNameException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Subscribe Share Name Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Subscribe Share Name Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeShareNameException|x|x|||| subscribeShareNameExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Share Name Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeShareNameExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse>> CreateMsgVpnAclProfileSubscribeShareNameExceptionAsyncWithHttpInfo (MsgVpnAclProfileSubscribeShareNameException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeTopicException|x|x|||| subscribeTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeTopicExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeTopicExceptionResponse> CreateMsgVpnAclProfileSubscribeTopicExceptionAsync (MsgVpnAclProfileSubscribeTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeTopicException|x|x|||| subscribeTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeTopicExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse>> CreateMsgVpnAclProfileSubscribeTopicExceptionAsyncWithHttpInfo (MsgVpnAclProfileSubscribeTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Delete an ACL Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfileAsync (string msgVpnName, string aclProfileName);

        /// <summary>
        /// Delete an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Delete an ACL Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfileAsyncWithHttpInfo (string msgVpnName, string aclProfileName);
        /// <summary>
        /// Delete a Client Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Client Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfileClientConnectExceptionAsync (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress);

        /// <summary>
        /// Delete a Client Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Client Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfileClientConnectExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress);
        /// <summary>
        /// Delete a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfilePublishExceptionAsync (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic);

        /// <summary>
        /// Delete a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfilePublishExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic);
        /// <summary>
        /// Delete a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfilePublishTopicExceptionAsync (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException);

        /// <summary>
        /// Delete a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfilePublishTopicExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException);
        /// <summary>
        /// Delete a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfileSubscribeExceptionAsync (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic);

        /// <summary>
        /// Delete a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfileSubscribeExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic);
        /// <summary>
        /// Delete a Subscribe Share Name Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Subscribe Share Name Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfileSubscribeShareNameExceptionAsync (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException);

        /// <summary>
        /// Delete a Subscribe Share Name Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Subscribe Share Name Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfileSubscribeShareNameExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException);
        /// <summary>
        /// Delete a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfileSubscribeTopicExceptionAsync (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException);

        /// <summary>
        /// Delete a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfileSubscribeTopicExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException);
        /// <summary>
        /// Get an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Get an ACL Profile object.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileResponse> GetMsgVpnAclProfileAsync (string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Get an ACL Profile object.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileResponse>> GetMsgVpnAclProfileAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Client Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Client Connect Exception object.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileClientConnectExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileClientConnectExceptionResponse> GetMsgVpnAclProfileClientConnectExceptionAsync (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Client Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Client Connect Exception object.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileClientConnectExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse>> GetMsgVpnAclProfileClientConnectExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Client Connect Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Client Connect Exception objects.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileClientConnectExceptionsResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileClientConnectExceptionsResponse> GetMsgVpnAclProfileClientConnectExceptionsAsync (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Client Connect Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Client Connect Exception objects.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileClientConnectExceptionsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileClientConnectExceptionsResponse>> GetMsgVpnAclProfileClientConnectExceptionsAsyncWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilePublishExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfilePublishExceptionResponse> GetMsgVpnAclProfilePublishExceptionAsync (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilePublishExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilePublishExceptionResponse>> GetMsgVpnAclProfilePublishExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Publish Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilePublishExceptionsResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfilePublishExceptionsResponse> GetMsgVpnAclProfilePublishExceptionsAsync (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Publish Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilePublishExceptionsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilePublishExceptionsResponse>> GetMsgVpnAclProfilePublishExceptionsAsyncWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilePublishTopicExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfilePublishTopicExceptionResponse> GetMsgVpnAclProfilePublishTopicExceptionAsync (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Publish Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilePublishTopicExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse>> GetMsgVpnAclProfilePublishTopicExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Publish Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilePublishTopicExceptionsResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfilePublishTopicExceptionsResponse> GetMsgVpnAclProfilePublishTopicExceptionsAsync (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Publish Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilePublishTopicExceptionsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilePublishTopicExceptionsResponse>> GetMsgVpnAclProfilePublishTopicExceptionsAsyncWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeExceptionResponse> GetMsgVpnAclProfileSubscribeExceptionAsync (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse>> GetMsgVpnAclProfileSubscribeExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Subscribe Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeExceptionsResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeExceptionsResponse> GetMsgVpnAclProfileSubscribeExceptionsAsync (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Subscribe Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeExceptionsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeExceptionsResponse>> GetMsgVpnAclProfileSubscribeExceptionsAsyncWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Subscribe Share Name Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Subscribe Share Name Exception object.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeShareNameExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeShareNameExceptionResponse> GetMsgVpnAclProfileSubscribeShareNameExceptionAsync (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Subscribe Share Name Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Subscribe Share Name Exception object.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeShareNameExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse>> GetMsgVpnAclProfileSubscribeShareNameExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Subscribe Share Name Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Subscribe Share Name Exception objects.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeShareNameExceptionsResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeShareNameExceptionsResponse> GetMsgVpnAclProfileSubscribeShareNameExceptionsAsync (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Subscribe Share Name Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Subscribe Share Name Exception objects.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeShareNameExceptionsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionsResponse>> GetMsgVpnAclProfileSubscribeShareNameExceptionsAsyncWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeTopicExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeTopicExceptionResponse> GetMsgVpnAclProfileSubscribeTopicExceptionAsync (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Subscribe Topic Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeTopicExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse>> GetMsgVpnAclProfileSubscribeTopicExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Subscribe Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeTopicExceptionsResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeTopicExceptionsResponse> GetMsgVpnAclProfileSubscribeTopicExceptionsAsync (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Subscribe Topic Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeTopicExceptionsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionsResponse>> GetMsgVpnAclProfileSubscribeTopicExceptionsAsyncWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of ACL Profile objects.
        /// </summary>
        /// <remarks>
        /// Get a list of ACL Profile objects.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilesResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfilesResponse> GetMsgVpnAclProfilesAsync (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of ACL Profile objects.
        /// </summary>
        /// <remarks>
        /// Get a list of ACL Profile objects.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilesResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilesResponse>> GetMsgVpnAclProfilesAsyncWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Replace an ACL Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileResponse> ReplaceMsgVpnAclProfileAsync (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Replace an ACL Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileResponse>> ReplaceMsgVpnAclProfileAsyncWithHttpInfo (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Update an ACL Profile object. Any attribute missing from the request will be left unchanged.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileResponse</returns>
        System.Threading.Tasks.Task<MsgVpnAclProfileResponse> UpdateMsgVpnAclProfileAsync (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update an ACL Profile object.
        /// </summary>
        /// <remarks>
        /// Update an ACL Profile object. Any attribute missing from the request will be left unchanged.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileResponse>> UpdateMsgVpnAclProfileAsyncWithHttpInfo (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public partial class AclProfileApi : IAclProfileApi
    {
        private Semp.V2.CSharp.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AclProfileApi"/> class.
        /// </summary>
        /// <returns></returns>
        public AclProfileApi(String basePath)
        {
            this.Configuration = new Semp.V2.CSharp.Client.Configuration { BasePath = basePath };

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AclProfileApi"/> class
        /// </summary>
        /// <returns></returns>
        public AclProfileApi()
        {
            this.Configuration = Semp.V2.CSharp.Client.Configuration.Default;

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AclProfileApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public AclProfileApi(Semp.V2.CSharp.Client.Configuration configuration = null)
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
        /// Create an ACL Profile object. Create an ACL Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileResponse</returns>
        public MsgVpnAclProfileResponse CreateMsgVpnAclProfile (MsgVpnAclProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileResponse> localVarResponse = CreateMsgVpnAclProfileWithHttpInfo(body, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create an ACL Profile object. Create an ACL Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileResponse</returns>
        public ApiResponse< MsgVpnAclProfileResponse > CreateMsgVpnAclProfileWithHttpInfo (MsgVpnAclProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfile");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles";
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
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileResponse)));
        }

        /// <summary>
        /// Create an ACL Profile object. Create an ACL Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileResponse> CreateMsgVpnAclProfileAsync (MsgVpnAclProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileResponse> localVarResponse = await CreateMsgVpnAclProfileAsyncWithHttpInfo(body, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create an ACL Profile object. Create an ACL Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileResponse>> CreateMsgVpnAclProfileAsyncWithHttpInfo (MsgVpnAclProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfile");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles";
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
                Method.POST, localVarQueryParams, localVarPostBody, localVarHeaderParams, localVarFormParams, localVarFileParams,
                localVarPathParams, localVarHttpContentType);

            int localVarStatusCode = (int) localVarResponse.StatusCode;

            if (ExceptionFactory != null)
            {
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileResponse)));
        }

        /// <summary>
        /// Create a Client Connect Exception object. Create a Client Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| clientConnectExceptionAddress|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Client Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileClientConnectExceptionResponse</returns>
        public MsgVpnAclProfileClientConnectExceptionResponse CreateMsgVpnAclProfileClientConnectException (MsgVpnAclProfileClientConnectException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse> localVarResponse = CreateMsgVpnAclProfileClientConnectExceptionWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Client Connect Exception object. Create a Client Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| clientConnectExceptionAddress|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Client Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileClientConnectExceptionResponse</returns>
        public ApiResponse< MsgVpnAclProfileClientConnectExceptionResponse > CreateMsgVpnAclProfileClientConnectExceptionWithHttpInfo (MsgVpnAclProfileClientConnectException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfileClientConnectException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfileClientConnectException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->CreateMsgVpnAclProfileClientConnectException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfileClientConnectException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileClientConnectExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileClientConnectExceptionResponse)));
        }

        /// <summary>
        /// Create a Client Connect Exception object. Create a Client Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| clientConnectExceptionAddress|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Client Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileClientConnectExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileClientConnectExceptionResponse> CreateMsgVpnAclProfileClientConnectExceptionAsync (MsgVpnAclProfileClientConnectException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse> localVarResponse = await CreateMsgVpnAclProfileClientConnectExceptionAsyncWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Client Connect Exception object. Create a Client Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| clientConnectExceptionAddress|x|x|||| msgVpnName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Client Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileClientConnectExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse>> CreateMsgVpnAclProfileClientConnectExceptionAsyncWithHttpInfo (MsgVpnAclProfileClientConnectException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfileClientConnectException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfileClientConnectException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->CreateMsgVpnAclProfileClientConnectException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfileClientConnectException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileClientConnectExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileClientConnectExceptionResponse)));
        }

        /// <summary>
        /// Create a Publish Topic Exception object. Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| publishExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilePublishExceptionResponse</returns>
        public MsgVpnAclProfilePublishExceptionResponse CreateMsgVpnAclProfilePublishException (MsgVpnAclProfilePublishException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilePublishExceptionResponse> localVarResponse = CreateMsgVpnAclProfilePublishExceptionWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Publish Topic Exception object. Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| publishExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilePublishExceptionResponse</returns>
        public ApiResponse< MsgVpnAclProfilePublishExceptionResponse > CreateMsgVpnAclProfilePublishExceptionWithHttpInfo (MsgVpnAclProfilePublishException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfilePublishException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfilePublishException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->CreateMsgVpnAclProfilePublishException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfilePublishException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilePublishExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilePublishExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilePublishExceptionResponse)));
        }

        /// <summary>
        /// Create a Publish Topic Exception object. Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| publishExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilePublishExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfilePublishExceptionResponse> CreateMsgVpnAclProfilePublishExceptionAsync (MsgVpnAclProfilePublishException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilePublishExceptionResponse> localVarResponse = await CreateMsgVpnAclProfilePublishExceptionAsyncWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Publish Topic Exception object. Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| publishExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilePublishExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilePublishExceptionResponse>> CreateMsgVpnAclProfilePublishExceptionAsyncWithHttpInfo (MsgVpnAclProfilePublishException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfilePublishException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfilePublishException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->CreateMsgVpnAclProfilePublishException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfilePublishException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilePublishExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilePublishExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilePublishExceptionResponse)));
        }

        /// <summary>
        /// Create a Publish Topic Exception object. Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| publishTopicException|x|x|||| publishTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilePublishTopicExceptionResponse</returns>
        public MsgVpnAclProfilePublishTopicExceptionResponse CreateMsgVpnAclProfilePublishTopicException (MsgVpnAclProfilePublishTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse> localVarResponse = CreateMsgVpnAclProfilePublishTopicExceptionWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Publish Topic Exception object. Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| publishTopicException|x|x|||| publishTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilePublishTopicExceptionResponse</returns>
        public ApiResponse< MsgVpnAclProfilePublishTopicExceptionResponse > CreateMsgVpnAclProfilePublishTopicExceptionWithHttpInfo (MsgVpnAclProfilePublishTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->CreateMsgVpnAclProfilePublishTopicException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfilePublishTopicException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilePublishTopicExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilePublishTopicExceptionResponse)));
        }

        /// <summary>
        /// Create a Publish Topic Exception object. Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| publishTopicException|x|x|||| publishTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilePublishTopicExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfilePublishTopicExceptionResponse> CreateMsgVpnAclProfilePublishTopicExceptionAsync (MsgVpnAclProfilePublishTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse> localVarResponse = await CreateMsgVpnAclProfilePublishTopicExceptionAsyncWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Publish Topic Exception object. Create a Publish Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| publishTopicException|x|x|||| publishTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Publish Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilePublishTopicExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse>> CreateMsgVpnAclProfilePublishTopicExceptionAsyncWithHttpInfo (MsgVpnAclProfilePublishTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->CreateMsgVpnAclProfilePublishTopicException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfilePublishTopicException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilePublishTopicExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilePublishTopicExceptionResponse)));
        }

        /// <summary>
        /// Create a Subscribe Topic Exception object. Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| subscribeExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeExceptionResponse</returns>
        public MsgVpnAclProfileSubscribeExceptionResponse CreateMsgVpnAclProfileSubscribeException (MsgVpnAclProfileSubscribeException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse> localVarResponse = CreateMsgVpnAclProfileSubscribeExceptionWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Subscribe Topic Exception object. Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| subscribeExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeExceptionResponse</returns>
        public ApiResponse< MsgVpnAclProfileSubscribeExceptionResponse > CreateMsgVpnAclProfileSubscribeExceptionWithHttpInfo (MsgVpnAclProfileSubscribeException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfileSubscribeException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeExceptionResponse)));
        }

        /// <summary>
        /// Create a Subscribe Topic Exception object. Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| subscribeExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeExceptionResponse> CreateMsgVpnAclProfileSubscribeExceptionAsync (MsgVpnAclProfileSubscribeException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse> localVarResponse = await CreateMsgVpnAclProfileSubscribeExceptionAsyncWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Subscribe Topic Exception object. Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||x| msgVpnName|x||x||x| subscribeExceptionTopic|x|x|||x| topicSyntax|x|x|||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse>> CreateMsgVpnAclProfileSubscribeExceptionAsyncWithHttpInfo (MsgVpnAclProfileSubscribeException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfileSubscribeException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeExceptionResponse)));
        }

        /// <summary>
        /// Create a Subscribe Share Name Exception object. Create a Subscribe Share Name Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeShareNameException|x|x|||| subscribeShareNameExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Share Name Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeShareNameExceptionResponse</returns>
        public MsgVpnAclProfileSubscribeShareNameExceptionResponse CreateMsgVpnAclProfileSubscribeShareNameException (MsgVpnAclProfileSubscribeShareNameException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse> localVarResponse = CreateMsgVpnAclProfileSubscribeShareNameExceptionWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Subscribe Share Name Exception object. Create a Subscribe Share Name Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeShareNameException|x|x|||| subscribeShareNameExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Share Name Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeShareNameExceptionResponse</returns>
        public ApiResponse< MsgVpnAclProfileSubscribeShareNameExceptionResponse > CreateMsgVpnAclProfileSubscribeShareNameExceptionWithHttpInfo (MsgVpnAclProfileSubscribeShareNameException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeShareNameException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfileSubscribeShareNameException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeShareNameExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeShareNameExceptionResponse)));
        }

        /// <summary>
        /// Create a Subscribe Share Name Exception object. Create a Subscribe Share Name Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeShareNameException|x|x|||| subscribeShareNameExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Share Name Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeShareNameExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeShareNameExceptionResponse> CreateMsgVpnAclProfileSubscribeShareNameExceptionAsync (MsgVpnAclProfileSubscribeShareNameException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse> localVarResponse = await CreateMsgVpnAclProfileSubscribeShareNameExceptionAsyncWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Subscribe Share Name Exception object. Create a Subscribe Share Name Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeShareNameException|x|x|||| subscribeShareNameExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Share Name Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeShareNameExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse>> CreateMsgVpnAclProfileSubscribeShareNameExceptionAsyncWithHttpInfo (MsgVpnAclProfileSubscribeShareNameException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeShareNameException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfileSubscribeShareNameException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeShareNameExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeShareNameExceptionResponse)));
        }

        /// <summary>
        /// Create a Subscribe Topic Exception object. Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeTopicException|x|x|||| subscribeTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeTopicExceptionResponse</returns>
        public MsgVpnAclProfileSubscribeTopicExceptionResponse CreateMsgVpnAclProfileSubscribeTopicException (MsgVpnAclProfileSubscribeTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse> localVarResponse = CreateMsgVpnAclProfileSubscribeTopicExceptionWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Subscribe Topic Exception object. Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeTopicException|x|x|||| subscribeTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeTopicExceptionResponse</returns>
        public ApiResponse< MsgVpnAclProfileSubscribeTopicExceptionResponse > CreateMsgVpnAclProfileSubscribeTopicExceptionWithHttpInfo (MsgVpnAclProfileSubscribeTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeTopicException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfileSubscribeTopicException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeTopicExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeTopicExceptionResponse)));
        }

        /// <summary>
        /// Create a Subscribe Topic Exception object. Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeTopicException|x|x|||| subscribeTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeTopicExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeTopicExceptionResponse> CreateMsgVpnAclProfileSubscribeTopicExceptionAsync (MsgVpnAclProfileSubscribeTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse> localVarResponse = await CreateMsgVpnAclProfileSubscribeTopicExceptionAsyncWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Subscribe Topic Exception object. Create a Subscribe Topic Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x||| msgVpnName|x||x||| subscribeTopicException|x|x|||| subscribeTopicExceptionSyntax|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Subscribe Topic Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeTopicExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse>> CreateMsgVpnAclProfileSubscribeTopicExceptionAsyncWithHttpInfo (MsgVpnAclProfileSubscribeTopicException body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->CreateMsgVpnAclProfileSubscribeTopicException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnAclProfileSubscribeTopicException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeTopicExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeTopicExceptionResponse)));
        }

        /// <summary>
        /// Delete an ACL Profile object. Delete an ACL Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnAclProfile (string msgVpnName, string aclProfileName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnAclProfileWithHttpInfo(msgVpnName, aclProfileName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete an ACL Profile object. Delete an ACL Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnAclProfileWithHttpInfo (string msgVpnName, string aclProfileName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfile");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete an ACL Profile object. Delete an ACL Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfileAsync (string msgVpnName, string aclProfileName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnAclProfileAsyncWithHttpInfo(msgVpnName, aclProfileName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete an ACL Profile object. Delete an ACL Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfileAsyncWithHttpInfo (string msgVpnName, string aclProfileName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfile");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Client Connect Exception object. Delete a Client Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnAclProfileClientConnectException (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnAclProfileClientConnectExceptionWithHttpInfo(msgVpnName, aclProfileName, clientConnectExceptionAddress);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Client Connect Exception object. Delete a Client Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnAclProfileClientConnectExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfileClientConnectException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfileClientConnectException");
            // verify the required parameter 'clientConnectExceptionAddress' is set
            if (clientConnectExceptionAddress == null)
                throw new ApiException(400, "Missing required parameter 'clientConnectExceptionAddress' when calling AclProfileApi->DeleteMsgVpnAclProfileClientConnectException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions/{clientConnectExceptionAddress}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (clientConnectExceptionAddress != null) localVarPathParams.Add("clientConnectExceptionAddress", this.Configuration.ApiClient.ParameterToString(clientConnectExceptionAddress)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfileClientConnectException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Client Connect Exception object. Delete a Client Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfileClientConnectExceptionAsync (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnAclProfileClientConnectExceptionAsyncWithHttpInfo(msgVpnName, aclProfileName, clientConnectExceptionAddress);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Client Connect Exception object. Delete a Client Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfileClientConnectExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfileClientConnectException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfileClientConnectException");
            // verify the required parameter 'clientConnectExceptionAddress' is set
            if (clientConnectExceptionAddress == null)
                throw new ApiException(400, "Missing required parameter 'clientConnectExceptionAddress' when calling AclProfileApi->DeleteMsgVpnAclProfileClientConnectException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions/{clientConnectExceptionAddress}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (clientConnectExceptionAddress != null) localVarPathParams.Add("clientConnectExceptionAddress", this.Configuration.ApiClient.ParameterToString(clientConnectExceptionAddress)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfileClientConnectException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Publish Topic Exception object. Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnAclProfilePublishException (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnAclProfilePublishExceptionWithHttpInfo(msgVpnName, aclProfileName, topicSyntax, publishExceptionTopic);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Publish Topic Exception object. Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnAclProfilePublishExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishException");
            // verify the required parameter 'topicSyntax' is set
            if (topicSyntax == null)
                throw new ApiException(400, "Missing required parameter 'topicSyntax' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishException");
            // verify the required parameter 'publishExceptionTopic' is set
            if (publishExceptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'publishExceptionTopic' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions/{topicSyntax},{publishExceptionTopic}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (topicSyntax != null) localVarPathParams.Add("topicSyntax", this.Configuration.ApiClient.ParameterToString(topicSyntax)); // path parameter
            if (publishExceptionTopic != null) localVarPathParams.Add("publishExceptionTopic", this.Configuration.ApiClient.ParameterToString(publishExceptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfilePublishException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Publish Topic Exception object. Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfilePublishExceptionAsync (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnAclProfilePublishExceptionAsyncWithHttpInfo(msgVpnName, aclProfileName, topicSyntax, publishExceptionTopic);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Publish Topic Exception object. Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfilePublishExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishException");
            // verify the required parameter 'topicSyntax' is set
            if (topicSyntax == null)
                throw new ApiException(400, "Missing required parameter 'topicSyntax' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishException");
            // verify the required parameter 'publishExceptionTopic' is set
            if (publishExceptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'publishExceptionTopic' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions/{topicSyntax},{publishExceptionTopic}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (topicSyntax != null) localVarPathParams.Add("topicSyntax", this.Configuration.ApiClient.ParameterToString(topicSyntax)); // path parameter
            if (publishExceptionTopic != null) localVarPathParams.Add("publishExceptionTopic", this.Configuration.ApiClient.ParameterToString(publishExceptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfilePublishException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Publish Topic Exception object. Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnAclProfilePublishTopicException (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnAclProfilePublishTopicExceptionWithHttpInfo(msgVpnName, aclProfileName, publishTopicExceptionSyntax, publishTopicException);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Publish Topic Exception object. Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnAclProfilePublishTopicExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'publishTopicExceptionSyntax' is set
            if (publishTopicExceptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'publishTopicExceptionSyntax' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'publishTopicException' is set
            if (publishTopicException == null)
                throw new ApiException(400, "Missing required parameter 'publishTopicException' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishTopicException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions/{publishTopicExceptionSyntax},{publishTopicException}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (publishTopicExceptionSyntax != null) localVarPathParams.Add("publishTopicExceptionSyntax", this.Configuration.ApiClient.ParameterToString(publishTopicExceptionSyntax)); // path parameter
            if (publishTopicException != null) localVarPathParams.Add("publishTopicException", this.Configuration.ApiClient.ParameterToString(publishTopicException)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfilePublishTopicException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Publish Topic Exception object. Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfilePublishTopicExceptionAsync (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnAclProfilePublishTopicExceptionAsyncWithHttpInfo(msgVpnName, aclProfileName, publishTopicExceptionSyntax, publishTopicException);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Publish Topic Exception object. Delete a Publish Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfilePublishTopicExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'publishTopicExceptionSyntax' is set
            if (publishTopicExceptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'publishTopicExceptionSyntax' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'publishTopicException' is set
            if (publishTopicException == null)
                throw new ApiException(400, "Missing required parameter 'publishTopicException' when calling AclProfileApi->DeleteMsgVpnAclProfilePublishTopicException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions/{publishTopicExceptionSyntax},{publishTopicException}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (publishTopicExceptionSyntax != null) localVarPathParams.Add("publishTopicExceptionSyntax", this.Configuration.ApiClient.ParameterToString(publishTopicExceptionSyntax)); // path parameter
            if (publishTopicException != null) localVarPathParams.Add("publishTopicException", this.Configuration.ApiClient.ParameterToString(publishTopicException)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfilePublishTopicException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Subscribe Topic Exception object. Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnAclProfileSubscribeException (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnAclProfileSubscribeExceptionWithHttpInfo(msgVpnName, aclProfileName, topicSyntax, subscribeExceptionTopic);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Subscribe Topic Exception object. Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnAclProfileSubscribeExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'topicSyntax' is set
            if (topicSyntax == null)
                throw new ApiException(400, "Missing required parameter 'topicSyntax' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'subscribeExceptionTopic' is set
            if (subscribeExceptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'subscribeExceptionTopic' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions/{topicSyntax},{subscribeExceptionTopic}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (topicSyntax != null) localVarPathParams.Add("topicSyntax", this.Configuration.ApiClient.ParameterToString(topicSyntax)); // path parameter
            if (subscribeExceptionTopic != null) localVarPathParams.Add("subscribeExceptionTopic", this.Configuration.ApiClient.ParameterToString(subscribeExceptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfileSubscribeException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Subscribe Topic Exception object. Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfileSubscribeExceptionAsync (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnAclProfileSubscribeExceptionAsyncWithHttpInfo(msgVpnName, aclProfileName, topicSyntax, subscribeExceptionTopic);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Subscribe Topic Exception object. Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfileSubscribeExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'topicSyntax' is set
            if (topicSyntax == null)
                throw new ApiException(400, "Missing required parameter 'topicSyntax' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'subscribeExceptionTopic' is set
            if (subscribeExceptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'subscribeExceptionTopic' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions/{topicSyntax},{subscribeExceptionTopic}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (topicSyntax != null) localVarPathParams.Add("topicSyntax", this.Configuration.ApiClient.ParameterToString(topicSyntax)); // path parameter
            if (subscribeExceptionTopic != null) localVarPathParams.Add("subscribeExceptionTopic", this.Configuration.ApiClient.ParameterToString(subscribeExceptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfileSubscribeException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Subscribe Share Name Exception object. Delete a Subscribe Share Name Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnAclProfileSubscribeShareNameException (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnAclProfileSubscribeShareNameExceptionWithHttpInfo(msgVpnName, aclProfileName, subscribeShareNameExceptionSyntax, subscribeShareNameException);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Subscribe Share Name Exception object. Delete a Subscribe Share Name Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnAclProfileSubscribeShareNameExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'subscribeShareNameExceptionSyntax' is set
            if (subscribeShareNameExceptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'subscribeShareNameExceptionSyntax' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'subscribeShareNameException' is set
            if (subscribeShareNameException == null)
                throw new ApiException(400, "Missing required parameter 'subscribeShareNameException' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeShareNameException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions/{subscribeShareNameExceptionSyntax},{subscribeShareNameException}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (subscribeShareNameExceptionSyntax != null) localVarPathParams.Add("subscribeShareNameExceptionSyntax", this.Configuration.ApiClient.ParameterToString(subscribeShareNameExceptionSyntax)); // path parameter
            if (subscribeShareNameException != null) localVarPathParams.Add("subscribeShareNameException", this.Configuration.ApiClient.ParameterToString(subscribeShareNameException)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfileSubscribeShareNameException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Subscribe Share Name Exception object. Delete a Subscribe Share Name Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfileSubscribeShareNameExceptionAsync (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnAclProfileSubscribeShareNameExceptionAsyncWithHttpInfo(msgVpnName, aclProfileName, subscribeShareNameExceptionSyntax, subscribeShareNameException);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Subscribe Share Name Exception object. Delete a Subscribe Share Name Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfileSubscribeShareNameExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'subscribeShareNameExceptionSyntax' is set
            if (subscribeShareNameExceptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'subscribeShareNameExceptionSyntax' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'subscribeShareNameException' is set
            if (subscribeShareNameException == null)
                throw new ApiException(400, "Missing required parameter 'subscribeShareNameException' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeShareNameException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions/{subscribeShareNameExceptionSyntax},{subscribeShareNameException}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (subscribeShareNameExceptionSyntax != null) localVarPathParams.Add("subscribeShareNameExceptionSyntax", this.Configuration.ApiClient.ParameterToString(subscribeShareNameExceptionSyntax)); // path parameter
            if (subscribeShareNameException != null) localVarPathParams.Add("subscribeShareNameException", this.Configuration.ApiClient.ParameterToString(subscribeShareNameException)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfileSubscribeShareNameException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Subscribe Topic Exception object. Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnAclProfileSubscribeTopicException (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnAclProfileSubscribeTopicExceptionWithHttpInfo(msgVpnName, aclProfileName, subscribeTopicExceptionSyntax, subscribeTopicException);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Subscribe Topic Exception object. Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnAclProfileSubscribeTopicExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'subscribeTopicExceptionSyntax' is set
            if (subscribeTopicExceptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'subscribeTopicExceptionSyntax' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'subscribeTopicException' is set
            if (subscribeTopicException == null)
                throw new ApiException(400, "Missing required parameter 'subscribeTopicException' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeTopicException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions/{subscribeTopicExceptionSyntax},{subscribeTopicException}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (subscribeTopicExceptionSyntax != null) localVarPathParams.Add("subscribeTopicExceptionSyntax", this.Configuration.ApiClient.ParameterToString(subscribeTopicExceptionSyntax)); // path parameter
            if (subscribeTopicException != null) localVarPathParams.Add("subscribeTopicException", this.Configuration.ApiClient.ParameterToString(subscribeTopicException)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfileSubscribeTopicException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Subscribe Topic Exception object. Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnAclProfileSubscribeTopicExceptionAsync (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnAclProfileSubscribeTopicExceptionAsyncWithHttpInfo(msgVpnName, aclProfileName, subscribeTopicExceptionSyntax, subscribeTopicException);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Subscribe Topic Exception object. Delete a Subscribe Topic Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnAclProfileSubscribeTopicExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'subscribeTopicExceptionSyntax' is set
            if (subscribeTopicExceptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'subscribeTopicExceptionSyntax' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'subscribeTopicException' is set
            if (subscribeTopicException == null)
                throw new ApiException(400, "Missing required parameter 'subscribeTopicException' when calling AclProfileApi->DeleteMsgVpnAclProfileSubscribeTopicException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions/{subscribeTopicExceptionSyntax},{subscribeTopicException}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (subscribeTopicExceptionSyntax != null) localVarPathParams.Add("subscribeTopicExceptionSyntax", this.Configuration.ApiClient.ParameterToString(subscribeTopicExceptionSyntax)); // path parameter
            if (subscribeTopicException != null) localVarPathParams.Add("subscribeTopicException", this.Configuration.ApiClient.ParameterToString(subscribeTopicException)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnAclProfileSubscribeTopicException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Get an ACL Profile object. Get an ACL Profile object.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileResponse</returns>
        public MsgVpnAclProfileResponse GetMsgVpnAclProfile (string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileResponse> localVarResponse = GetMsgVpnAclProfileWithHttpInfo(msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get an ACL Profile object. Get an ACL Profile object.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileResponse</returns>
        public ApiResponse< MsgVpnAclProfileResponse > GetMsgVpnAclProfileWithHttpInfo (string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfile");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileResponse)));
        }

        /// <summary>
        /// Get an ACL Profile object. Get an ACL Profile object.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileResponse> GetMsgVpnAclProfileAsync (string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileResponse> localVarResponse = await GetMsgVpnAclProfileAsyncWithHttpInfo(msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get an ACL Profile object. Get an ACL Profile object.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileResponse>> GetMsgVpnAclProfileAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfile");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileResponse)));
        }

        /// <summary>
        /// Get a Client Connect Exception object. Get a Client Connect Exception object.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileClientConnectExceptionResponse</returns>
        public MsgVpnAclProfileClientConnectExceptionResponse GetMsgVpnAclProfileClientConnectException (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse> localVarResponse = GetMsgVpnAclProfileClientConnectExceptionWithHttpInfo(msgVpnName, aclProfileName, clientConnectExceptionAddress, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Client Connect Exception object. Get a Client Connect Exception object.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileClientConnectExceptionResponse</returns>
        public ApiResponse< MsgVpnAclProfileClientConnectExceptionResponse > GetMsgVpnAclProfileClientConnectExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileClientConnectException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileClientConnectException");
            // verify the required parameter 'clientConnectExceptionAddress' is set
            if (clientConnectExceptionAddress == null)
                throw new ApiException(400, "Missing required parameter 'clientConnectExceptionAddress' when calling AclProfileApi->GetMsgVpnAclProfileClientConnectException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions/{clientConnectExceptionAddress}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (clientConnectExceptionAddress != null) localVarPathParams.Add("clientConnectExceptionAddress", this.Configuration.ApiClient.ParameterToString(clientConnectExceptionAddress)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileClientConnectException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileClientConnectExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileClientConnectExceptionResponse)));
        }

        /// <summary>
        /// Get a Client Connect Exception object. Get a Client Connect Exception object.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileClientConnectExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileClientConnectExceptionResponse> GetMsgVpnAclProfileClientConnectExceptionAsync (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse> localVarResponse = await GetMsgVpnAclProfileClientConnectExceptionAsyncWithHttpInfo(msgVpnName, aclProfileName, clientConnectExceptionAddress, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Client Connect Exception object. Get a Client Connect Exception object.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="clientConnectExceptionAddress">The IP address/netmask of the client connect exception in canonical CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileClientConnectExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse>> GetMsgVpnAclProfileClientConnectExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string clientConnectExceptionAddress, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileClientConnectException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileClientConnectException");
            // verify the required parameter 'clientConnectExceptionAddress' is set
            if (clientConnectExceptionAddress == null)
                throw new ApiException(400, "Missing required parameter 'clientConnectExceptionAddress' when calling AclProfileApi->GetMsgVpnAclProfileClientConnectException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions/{clientConnectExceptionAddress}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (clientConnectExceptionAddress != null) localVarPathParams.Add("clientConnectExceptionAddress", this.Configuration.ApiClient.ParameterToString(clientConnectExceptionAddress)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileClientConnectException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileClientConnectExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileClientConnectExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileClientConnectExceptionResponse)));
        }

        /// <summary>
        /// Get a list of Client Connect Exception objects. Get a list of Client Connect Exception objects.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileClientConnectExceptionsResponse</returns>
        public MsgVpnAclProfileClientConnectExceptionsResponse GetMsgVpnAclProfileClientConnectExceptions (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileClientConnectExceptionsResponse> localVarResponse = GetMsgVpnAclProfileClientConnectExceptionsWithHttpInfo(msgVpnName, aclProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Client Connect Exception objects. Get a list of Client Connect Exception objects.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileClientConnectExceptionsResponse</returns>
        public ApiResponse< MsgVpnAclProfileClientConnectExceptionsResponse > GetMsgVpnAclProfileClientConnectExceptionsWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileClientConnectExceptions");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileClientConnectExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileClientConnectExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileClientConnectExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileClientConnectExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileClientConnectExceptionsResponse)));
        }

        /// <summary>
        /// Get a list of Client Connect Exception objects. Get a list of Client Connect Exception objects.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileClientConnectExceptionsResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileClientConnectExceptionsResponse> GetMsgVpnAclProfileClientConnectExceptionsAsync (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileClientConnectExceptionsResponse> localVarResponse = await GetMsgVpnAclProfileClientConnectExceptionsAsyncWithHttpInfo(msgVpnName, aclProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Client Connect Exception objects. Get a list of Client Connect Exception objects.  A Client Connect Exception is an exception to the default action to take when a client using the ACL Profile connects to the Message VPN. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| clientConnectExceptionAddress|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileClientConnectExceptionsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileClientConnectExceptionsResponse>> GetMsgVpnAclProfileClientConnectExceptionsAsyncWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileClientConnectExceptions");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileClientConnectExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/clientConnectExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileClientConnectExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileClientConnectExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileClientConnectExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileClientConnectExceptionsResponse)));
        }

        /// <summary>
        /// Get a Publish Topic Exception object. Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilePublishExceptionResponse</returns>
        public MsgVpnAclProfilePublishExceptionResponse GetMsgVpnAclProfilePublishException (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilePublishExceptionResponse> localVarResponse = GetMsgVpnAclProfilePublishExceptionWithHttpInfo(msgVpnName, aclProfileName, topicSyntax, publishExceptionTopic, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Publish Topic Exception object. Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilePublishExceptionResponse</returns>
        public ApiResponse< MsgVpnAclProfilePublishExceptionResponse > GetMsgVpnAclProfilePublishExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfilePublishException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfilePublishException");
            // verify the required parameter 'topicSyntax' is set
            if (topicSyntax == null)
                throw new ApiException(400, "Missing required parameter 'topicSyntax' when calling AclProfileApi->GetMsgVpnAclProfilePublishException");
            // verify the required parameter 'publishExceptionTopic' is set
            if (publishExceptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'publishExceptionTopic' when calling AclProfileApi->GetMsgVpnAclProfilePublishException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions/{topicSyntax},{publishExceptionTopic}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (topicSyntax != null) localVarPathParams.Add("topicSyntax", this.Configuration.ApiClient.ParameterToString(topicSyntax)); // path parameter
            if (publishExceptionTopic != null) localVarPathParams.Add("publishExceptionTopic", this.Configuration.ApiClient.ParameterToString(publishExceptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfilePublishException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilePublishExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilePublishExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilePublishExceptionResponse)));
        }

        /// <summary>
        /// Get a Publish Topic Exception object. Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilePublishExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfilePublishExceptionResponse> GetMsgVpnAclProfilePublishExceptionAsync (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilePublishExceptionResponse> localVarResponse = await GetMsgVpnAclProfilePublishExceptionAsyncWithHttpInfo(msgVpnName, aclProfileName, topicSyntax, publishExceptionTopic, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Publish Topic Exception object. Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilePublishExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilePublishExceptionResponse>> GetMsgVpnAclProfilePublishExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string publishExceptionTopic, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfilePublishException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfilePublishException");
            // verify the required parameter 'topicSyntax' is set
            if (topicSyntax == null)
                throw new ApiException(400, "Missing required parameter 'topicSyntax' when calling AclProfileApi->GetMsgVpnAclProfilePublishException");
            // verify the required parameter 'publishExceptionTopic' is set
            if (publishExceptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'publishExceptionTopic' when calling AclProfileApi->GetMsgVpnAclProfilePublishException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions/{topicSyntax},{publishExceptionTopic}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (topicSyntax != null) localVarPathParams.Add("topicSyntax", this.Configuration.ApiClient.ParameterToString(topicSyntax)); // path parameter
            if (publishExceptionTopic != null) localVarPathParams.Add("publishExceptionTopic", this.Configuration.ApiClient.ParameterToString(publishExceptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfilePublishException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilePublishExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilePublishExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilePublishExceptionResponse)));
        }

        /// <summary>
        /// Get a list of Publish Topic Exception objects. Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilePublishExceptionsResponse</returns>
        public MsgVpnAclProfilePublishExceptionsResponse GetMsgVpnAclProfilePublishExceptions (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilePublishExceptionsResponse> localVarResponse = GetMsgVpnAclProfilePublishExceptionsWithHttpInfo(msgVpnName, aclProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Publish Topic Exception objects. Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilePublishExceptionsResponse</returns>
        public ApiResponse< MsgVpnAclProfilePublishExceptionsResponse > GetMsgVpnAclProfilePublishExceptionsWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfilePublishExceptions");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfilePublishExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfilePublishExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilePublishExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilePublishExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilePublishExceptionsResponse)));
        }

        /// <summary>
        /// Get a list of Publish Topic Exception objects. Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilePublishExceptionsResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfilePublishExceptionsResponse> GetMsgVpnAclProfilePublishExceptionsAsync (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilePublishExceptionsResponse> localVarResponse = await GetMsgVpnAclProfilePublishExceptionsAsyncWithHttpInfo(msgVpnName, aclProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Publish Topic Exception objects. Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| publishExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by publishTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilePublishExceptionsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilePublishExceptionsResponse>> GetMsgVpnAclProfilePublishExceptionsAsyncWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfilePublishExceptions");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfilePublishExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfilePublishExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilePublishExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilePublishExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilePublishExceptionsResponse)));
        }

        /// <summary>
        /// Get a Publish Topic Exception object. Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilePublishTopicExceptionResponse</returns>
        public MsgVpnAclProfilePublishTopicExceptionResponse GetMsgVpnAclProfilePublishTopicException (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse> localVarResponse = GetMsgVpnAclProfilePublishTopicExceptionWithHttpInfo(msgVpnName, aclProfileName, publishTopicExceptionSyntax, publishTopicException, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Publish Topic Exception object. Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilePublishTopicExceptionResponse</returns>
        public ApiResponse< MsgVpnAclProfilePublishTopicExceptionResponse > GetMsgVpnAclProfilePublishTopicExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'publishTopicExceptionSyntax' is set
            if (publishTopicExceptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'publishTopicExceptionSyntax' when calling AclProfileApi->GetMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'publishTopicException' is set
            if (publishTopicException == null)
                throw new ApiException(400, "Missing required parameter 'publishTopicException' when calling AclProfileApi->GetMsgVpnAclProfilePublishTopicException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions/{publishTopicExceptionSyntax},{publishTopicException}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (publishTopicExceptionSyntax != null) localVarPathParams.Add("publishTopicExceptionSyntax", this.Configuration.ApiClient.ParameterToString(publishTopicExceptionSyntax)); // path parameter
            if (publishTopicException != null) localVarPathParams.Add("publishTopicException", this.Configuration.ApiClient.ParameterToString(publishTopicException)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfilePublishTopicException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilePublishTopicExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilePublishTopicExceptionResponse)));
        }

        /// <summary>
        /// Get a Publish Topic Exception object. Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilePublishTopicExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfilePublishTopicExceptionResponse> GetMsgVpnAclProfilePublishTopicExceptionAsync (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse> localVarResponse = await GetMsgVpnAclProfilePublishTopicExceptionAsyncWithHttpInfo(msgVpnName, aclProfileName, publishTopicExceptionSyntax, publishTopicException, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Publish Topic Exception object. Get a Publish Topic Exception object.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="publishTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="publishTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilePublishTopicExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse>> GetMsgVpnAclProfilePublishTopicExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string publishTopicExceptionSyntax, string publishTopicException, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'publishTopicExceptionSyntax' is set
            if (publishTopicExceptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'publishTopicExceptionSyntax' when calling AclProfileApi->GetMsgVpnAclProfilePublishTopicException");
            // verify the required parameter 'publishTopicException' is set
            if (publishTopicException == null)
                throw new ApiException(400, "Missing required parameter 'publishTopicException' when calling AclProfileApi->GetMsgVpnAclProfilePublishTopicException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions/{publishTopicExceptionSyntax},{publishTopicException}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (publishTopicExceptionSyntax != null) localVarPathParams.Add("publishTopicExceptionSyntax", this.Configuration.ApiClient.ParameterToString(publishTopicExceptionSyntax)); // path parameter
            if (publishTopicException != null) localVarPathParams.Add("publishTopicException", this.Configuration.ApiClient.ParameterToString(publishTopicException)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfilePublishTopicException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilePublishTopicExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilePublishTopicExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilePublishTopicExceptionResponse)));
        }

        /// <summary>
        /// Get a list of Publish Topic Exception objects. Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilePublishTopicExceptionsResponse</returns>
        public MsgVpnAclProfilePublishTopicExceptionsResponse GetMsgVpnAclProfilePublishTopicExceptions (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilePublishTopicExceptionsResponse> localVarResponse = GetMsgVpnAclProfilePublishTopicExceptionsWithHttpInfo(msgVpnName, aclProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Publish Topic Exception objects. Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilePublishTopicExceptionsResponse</returns>
        public ApiResponse< MsgVpnAclProfilePublishTopicExceptionsResponse > GetMsgVpnAclProfilePublishTopicExceptionsWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfilePublishTopicExceptions");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfilePublishTopicExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfilePublishTopicExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilePublishTopicExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilePublishTopicExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilePublishTopicExceptionsResponse)));
        }

        /// <summary>
        /// Get a list of Publish Topic Exception objects. Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilePublishTopicExceptionsResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfilePublishTopicExceptionsResponse> GetMsgVpnAclProfilePublishTopicExceptionsAsync (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilePublishTopicExceptionsResponse> localVarResponse = await GetMsgVpnAclProfilePublishTopicExceptionsAsyncWithHttpInfo(msgVpnName, aclProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Publish Topic Exception objects. Get a list of Publish Topic Exception objects.  A Publish Topic Exception is an exception to the default action to take when a client using the ACL Profile publishes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| publishTopicException|x||| publishTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilePublishTopicExceptionsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilePublishTopicExceptionsResponse>> GetMsgVpnAclProfilePublishTopicExceptionsAsyncWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfilePublishTopicExceptions");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfilePublishTopicExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/publishTopicExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfilePublishTopicExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilePublishTopicExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilePublishTopicExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilePublishTopicExceptionsResponse)));
        }

        /// <summary>
        /// Get a Subscribe Topic Exception object. Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeExceptionResponse</returns>
        public MsgVpnAclProfileSubscribeExceptionResponse GetMsgVpnAclProfileSubscribeException (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse> localVarResponse = GetMsgVpnAclProfileSubscribeExceptionWithHttpInfo(msgVpnName, aclProfileName, topicSyntax, subscribeExceptionTopic, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Subscribe Topic Exception object. Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeExceptionResponse</returns>
        public ApiResponse< MsgVpnAclProfileSubscribeExceptionResponse > GetMsgVpnAclProfileSubscribeExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'topicSyntax' is set
            if (topicSyntax == null)
                throw new ApiException(400, "Missing required parameter 'topicSyntax' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'subscribeExceptionTopic' is set
            if (subscribeExceptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'subscribeExceptionTopic' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions/{topicSyntax},{subscribeExceptionTopic}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (topicSyntax != null) localVarPathParams.Add("topicSyntax", this.Configuration.ApiClient.ParameterToString(topicSyntax)); // path parameter
            if (subscribeExceptionTopic != null) localVarPathParams.Add("subscribeExceptionTopic", this.Configuration.ApiClient.ParameterToString(subscribeExceptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileSubscribeException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeExceptionResponse)));
        }

        /// <summary>
        /// Get a Subscribe Topic Exception object. Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeExceptionResponse> GetMsgVpnAclProfileSubscribeExceptionAsync (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse> localVarResponse = await GetMsgVpnAclProfileSubscribeExceptionAsyncWithHttpInfo(msgVpnName, aclProfileName, topicSyntax, subscribeExceptionTopic, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Subscribe Topic Exception object. Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="topicSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeExceptionTopic">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse>> GetMsgVpnAclProfileSubscribeExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string topicSyntax, string subscribeExceptionTopic, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'topicSyntax' is set
            if (topicSyntax == null)
                throw new ApiException(400, "Missing required parameter 'topicSyntax' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeException");
            // verify the required parameter 'subscribeExceptionTopic' is set
            if (subscribeExceptionTopic == null)
                throw new ApiException(400, "Missing required parameter 'subscribeExceptionTopic' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions/{topicSyntax},{subscribeExceptionTopic}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (topicSyntax != null) localVarPathParams.Add("topicSyntax", this.Configuration.ApiClient.ParameterToString(topicSyntax)); // path parameter
            if (subscribeExceptionTopic != null) localVarPathParams.Add("subscribeExceptionTopic", this.Configuration.ApiClient.ParameterToString(subscribeExceptionTopic)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileSubscribeException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeExceptionResponse)));
        }

        /// <summary>
        /// Get a list of Subscribe Topic Exception objects. Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeExceptionsResponse</returns>
        public MsgVpnAclProfileSubscribeExceptionsResponse GetMsgVpnAclProfileSubscribeExceptions (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeExceptionsResponse> localVarResponse = GetMsgVpnAclProfileSubscribeExceptionsWithHttpInfo(msgVpnName, aclProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Subscribe Topic Exception objects. Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeExceptionsResponse</returns>
        public ApiResponse< MsgVpnAclProfileSubscribeExceptionsResponse > GetMsgVpnAclProfileSubscribeExceptionsWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeExceptions");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileSubscribeExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeExceptionsResponse)));
        }

        /// <summary>
        /// Get a list of Subscribe Topic Exception objects. Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeExceptionsResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeExceptionsResponse> GetMsgVpnAclProfileSubscribeExceptionsAsync (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeExceptionsResponse> localVarResponse = await GetMsgVpnAclProfileSubscribeExceptionsAsyncWithHttpInfo(msgVpnName, aclProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Subscribe Topic Exception objects. Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||x| msgVpnName|x||x| subscribeExceptionTopic|x||x| topicSyntax|x||x|    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been deprecated since 2.14. Replaced by subscribeTopicExceptions.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeExceptionsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeExceptionsResponse>> GetMsgVpnAclProfileSubscribeExceptionsAsyncWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeExceptions");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileSubscribeExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeExceptionsResponse)));
        }

        /// <summary>
        /// Get a Subscribe Share Name Exception object. Get a Subscribe Share Name Exception object.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeShareNameExceptionResponse</returns>
        public MsgVpnAclProfileSubscribeShareNameExceptionResponse GetMsgVpnAclProfileSubscribeShareNameException (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse> localVarResponse = GetMsgVpnAclProfileSubscribeShareNameExceptionWithHttpInfo(msgVpnName, aclProfileName, subscribeShareNameExceptionSyntax, subscribeShareNameException, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Subscribe Share Name Exception object. Get a Subscribe Share Name Exception object.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeShareNameExceptionResponse</returns>
        public ApiResponse< MsgVpnAclProfileSubscribeShareNameExceptionResponse > GetMsgVpnAclProfileSubscribeShareNameExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'subscribeShareNameExceptionSyntax' is set
            if (subscribeShareNameExceptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'subscribeShareNameExceptionSyntax' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'subscribeShareNameException' is set
            if (subscribeShareNameException == null)
                throw new ApiException(400, "Missing required parameter 'subscribeShareNameException' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeShareNameException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions/{subscribeShareNameExceptionSyntax},{subscribeShareNameException}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (subscribeShareNameExceptionSyntax != null) localVarPathParams.Add("subscribeShareNameExceptionSyntax", this.Configuration.ApiClient.ParameterToString(subscribeShareNameExceptionSyntax)); // path parameter
            if (subscribeShareNameException != null) localVarPathParams.Add("subscribeShareNameException", this.Configuration.ApiClient.ParameterToString(subscribeShareNameException)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileSubscribeShareNameException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeShareNameExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeShareNameExceptionResponse)));
        }

        /// <summary>
        /// Get a Subscribe Share Name Exception object. Get a Subscribe Share Name Exception object.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeShareNameExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeShareNameExceptionResponse> GetMsgVpnAclProfileSubscribeShareNameExceptionAsync (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse> localVarResponse = await GetMsgVpnAclProfileSubscribeShareNameExceptionAsyncWithHttpInfo(msgVpnName, aclProfileName, subscribeShareNameExceptionSyntax, subscribeShareNameException, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Subscribe Share Name Exception object. Get a Subscribe Share Name Exception object.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeShareNameExceptionSyntax">The syntax of the subscribe share name for the exception to the default action taken.</param>
        /// <param name="subscribeShareNameException">The subscribe share name exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeShareNameExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse>> GetMsgVpnAclProfileSubscribeShareNameExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeShareNameExceptionSyntax, string subscribeShareNameException, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'subscribeShareNameExceptionSyntax' is set
            if (subscribeShareNameExceptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'subscribeShareNameExceptionSyntax' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeShareNameException");
            // verify the required parameter 'subscribeShareNameException' is set
            if (subscribeShareNameException == null)
                throw new ApiException(400, "Missing required parameter 'subscribeShareNameException' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeShareNameException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions/{subscribeShareNameExceptionSyntax},{subscribeShareNameException}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (subscribeShareNameExceptionSyntax != null) localVarPathParams.Add("subscribeShareNameExceptionSyntax", this.Configuration.ApiClient.ParameterToString(subscribeShareNameExceptionSyntax)); // path parameter
            if (subscribeShareNameException != null) localVarPathParams.Add("subscribeShareNameException", this.Configuration.ApiClient.ParameterToString(subscribeShareNameException)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileSubscribeShareNameException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeShareNameExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeShareNameExceptionResponse)));
        }

        /// <summary>
        /// Get a list of Subscribe Share Name Exception objects. Get a list of Subscribe Share Name Exception objects.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeShareNameExceptionsResponse</returns>
        public MsgVpnAclProfileSubscribeShareNameExceptionsResponse GetMsgVpnAclProfileSubscribeShareNameExceptions (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionsResponse> localVarResponse = GetMsgVpnAclProfileSubscribeShareNameExceptionsWithHttpInfo(msgVpnName, aclProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Subscribe Share Name Exception objects. Get a list of Subscribe Share Name Exception objects.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeShareNameExceptionsResponse</returns>
        public ApiResponse< MsgVpnAclProfileSubscribeShareNameExceptionsResponse > GetMsgVpnAclProfileSubscribeShareNameExceptionsWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeShareNameExceptions");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeShareNameExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileSubscribeShareNameExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeShareNameExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeShareNameExceptionsResponse)));
        }

        /// <summary>
        /// Get a list of Subscribe Share Name Exception objects. Get a list of Subscribe Share Name Exception objects.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeShareNameExceptionsResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeShareNameExceptionsResponse> GetMsgVpnAclProfileSubscribeShareNameExceptionsAsync (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionsResponse> localVarResponse = await GetMsgVpnAclProfileSubscribeShareNameExceptionsAsyncWithHttpInfo(msgVpnName, aclProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Subscribe Share Name Exception objects. Get a list of Subscribe Share Name Exception objects.  A Subscribe Share Name Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a share-name subscription in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeShareNameException|x||| subscribeShareNameExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeShareNameExceptionsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionsResponse>> GetMsgVpnAclProfileSubscribeShareNameExceptionsAsyncWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeShareNameExceptions");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeShareNameExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeShareNameExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileSubscribeShareNameExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeShareNameExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeShareNameExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeShareNameExceptionsResponse)));
        }

        /// <summary>
        /// Get a Subscribe Topic Exception object. Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeTopicExceptionResponse</returns>
        public MsgVpnAclProfileSubscribeTopicExceptionResponse GetMsgVpnAclProfileSubscribeTopicException (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse> localVarResponse = GetMsgVpnAclProfileSubscribeTopicExceptionWithHttpInfo(msgVpnName, aclProfileName, subscribeTopicExceptionSyntax, subscribeTopicException, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Subscribe Topic Exception object. Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeTopicExceptionResponse</returns>
        public ApiResponse< MsgVpnAclProfileSubscribeTopicExceptionResponse > GetMsgVpnAclProfileSubscribeTopicExceptionWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'subscribeTopicExceptionSyntax' is set
            if (subscribeTopicExceptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'subscribeTopicExceptionSyntax' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'subscribeTopicException' is set
            if (subscribeTopicException == null)
                throw new ApiException(400, "Missing required parameter 'subscribeTopicException' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeTopicException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions/{subscribeTopicExceptionSyntax},{subscribeTopicException}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (subscribeTopicExceptionSyntax != null) localVarPathParams.Add("subscribeTopicExceptionSyntax", this.Configuration.ApiClient.ParameterToString(subscribeTopicExceptionSyntax)); // path parameter
            if (subscribeTopicException != null) localVarPathParams.Add("subscribeTopicException", this.Configuration.ApiClient.ParameterToString(subscribeTopicException)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileSubscribeTopicException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeTopicExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeTopicExceptionResponse)));
        }

        /// <summary>
        /// Get a Subscribe Topic Exception object. Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeTopicExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeTopicExceptionResponse> GetMsgVpnAclProfileSubscribeTopicExceptionAsync (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse> localVarResponse = await GetMsgVpnAclProfileSubscribeTopicExceptionAsyncWithHttpInfo(msgVpnName, aclProfileName, subscribeTopicExceptionSyntax, subscribeTopicException, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Subscribe Topic Exception object. Get a Subscribe Topic Exception object.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="subscribeTopicExceptionSyntax">The syntax of the topic for the exception to the default action taken.</param>
        /// <param name="subscribeTopicException">The topic for the exception to the default action taken. May include wildcard characters.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeTopicExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse>> GetMsgVpnAclProfileSubscribeTopicExceptionAsyncWithHttpInfo (string msgVpnName, string aclProfileName, string subscribeTopicExceptionSyntax, string subscribeTopicException, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'subscribeTopicExceptionSyntax' is set
            if (subscribeTopicExceptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'subscribeTopicExceptionSyntax' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeTopicException");
            // verify the required parameter 'subscribeTopicException' is set
            if (subscribeTopicException == null)
                throw new ApiException(400, "Missing required parameter 'subscribeTopicException' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeTopicException");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions/{subscribeTopicExceptionSyntax},{subscribeTopicException}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
            if (subscribeTopicExceptionSyntax != null) localVarPathParams.Add("subscribeTopicExceptionSyntax", this.Configuration.ApiClient.ParameterToString(subscribeTopicExceptionSyntax)); // path parameter
            if (subscribeTopicException != null) localVarPathParams.Add("subscribeTopicException", this.Configuration.ApiClient.ParameterToString(subscribeTopicException)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileSubscribeTopicException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeTopicExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeTopicExceptionResponse)));
        }

        /// <summary>
        /// Get a list of Subscribe Topic Exception objects. Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileSubscribeTopicExceptionsResponse</returns>
        public MsgVpnAclProfileSubscribeTopicExceptionsResponse GetMsgVpnAclProfileSubscribeTopicExceptions (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionsResponse> localVarResponse = GetMsgVpnAclProfileSubscribeTopicExceptionsWithHttpInfo(msgVpnName, aclProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Subscribe Topic Exception objects. Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileSubscribeTopicExceptionsResponse</returns>
        public ApiResponse< MsgVpnAclProfileSubscribeTopicExceptionsResponse > GetMsgVpnAclProfileSubscribeTopicExceptionsWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeTopicExceptions");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeTopicExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileSubscribeTopicExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeTopicExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeTopicExceptionsResponse)));
        }

        /// <summary>
        /// Get a list of Subscribe Topic Exception objects. Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileSubscribeTopicExceptionsResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileSubscribeTopicExceptionsResponse> GetMsgVpnAclProfileSubscribeTopicExceptionsAsync (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionsResponse> localVarResponse = await GetMsgVpnAclProfileSubscribeTopicExceptionsAsyncWithHttpInfo(msgVpnName, aclProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Subscribe Topic Exception objects. Get a list of Subscribe Topic Exception objects.  A Subscribe Topic Exception is an exception to the default action to take when a client using the ACL Profile subscribes to a topic in the Message VPN. Exceptions must be expressed as a topic.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x||| subscribeTopicException|x||| subscribeTopicExceptionSyntax|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.14.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileSubscribeTopicExceptionsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionsResponse>> GetMsgVpnAclProfileSubscribeTopicExceptionsAsyncWithHttpInfo (string msgVpnName, string aclProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeTopicExceptions");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->GetMsgVpnAclProfileSubscribeTopicExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}/subscribeTopicExceptions";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfileSubscribeTopicExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileSubscribeTopicExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileSubscribeTopicExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileSubscribeTopicExceptionsResponse)));
        }

        /// <summary>
        /// Get a list of ACL Profile objects. Get a list of ACL Profile objects.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfilesResponse</returns>
        public MsgVpnAclProfilesResponse GetMsgVpnAclProfiles (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilesResponse> localVarResponse = GetMsgVpnAclProfilesWithHttpInfo(msgVpnName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of ACL Profile objects. Get a list of ACL Profile objects.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfilesResponse</returns>
        public ApiResponse< MsgVpnAclProfilesResponse > GetMsgVpnAclProfilesWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfiles");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfiles", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilesResponse)));
        }

        /// <summary>
        /// Get a list of ACL Profile objects. Get a list of ACL Profile objects.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfilesResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfilesResponse> GetMsgVpnAclProfilesAsync (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfilesResponse> localVarResponse = await GetMsgVpnAclProfilesAsyncWithHttpInfo(msgVpnName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of ACL Profile objects. Get a list of ACL Profile objects.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: aclProfileName|x||| msgVpnName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfilesResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfilesResponse>> GetMsgVpnAclProfilesAsyncWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->GetMsgVpnAclProfiles");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnAclProfiles", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfilesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfilesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfilesResponse)));
        }

        /// <summary>
        /// Replace an ACL Profile object. Replace an ACL Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileResponse</returns>
        public MsgVpnAclProfileResponse ReplaceMsgVpnAclProfile (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileResponse> localVarResponse = ReplaceMsgVpnAclProfileWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace an ACL Profile object. Replace an ACL Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileResponse</returns>
        public ApiResponse< MsgVpnAclProfileResponse > ReplaceMsgVpnAclProfileWithHttpInfo (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->ReplaceMsgVpnAclProfile");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->ReplaceMsgVpnAclProfile");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->ReplaceMsgVpnAclProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("ReplaceMsgVpnAclProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileResponse)));
        }

        /// <summary>
        /// Replace an ACL Profile object. Replace an ACL Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileResponse> ReplaceMsgVpnAclProfileAsync (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileResponse> localVarResponse = await ReplaceMsgVpnAclProfileAsyncWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace an ACL Profile object. Replace an ACL Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x||x||||| msgVpnName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileResponse>> ReplaceMsgVpnAclProfileAsyncWithHttpInfo (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->ReplaceMsgVpnAclProfile");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->ReplaceMsgVpnAclProfile");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->ReplaceMsgVpnAclProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("ReplaceMsgVpnAclProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileResponse)));
        }

        /// <summary>
        /// Update an ACL Profile object. Update an ACL Profile object. Any attribute missing from the request will be left unchanged.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnAclProfileResponse</returns>
        public MsgVpnAclProfileResponse UpdateMsgVpnAclProfile (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileResponse> localVarResponse = UpdateMsgVpnAclProfileWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update an ACL Profile object. Update an ACL Profile object. Any attribute missing from the request will be left unchanged.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnAclProfileResponse</returns>
        public ApiResponse< MsgVpnAclProfileResponse > UpdateMsgVpnAclProfileWithHttpInfo (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->UpdateMsgVpnAclProfile");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->UpdateMsgVpnAclProfile");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->UpdateMsgVpnAclProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("UpdateMsgVpnAclProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileResponse)));
        }

        /// <summary>
        /// Update an ACL Profile object. Update an ACL Profile object. Any attribute missing from the request will be left unchanged.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnAclProfileResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnAclProfileResponse> UpdateMsgVpnAclProfileAsync (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnAclProfileResponse> localVarResponse = await UpdateMsgVpnAclProfileAsyncWithHttpInfo(body, msgVpnName, aclProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update an ACL Profile object. Update an ACL Profile object. Any attribute missing from the request will be left unchanged.  An ACL Profile controls whether an authenticated client is permitted to establish a connection with the message broker or permitted to publish and subscribe to specific topics.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- aclProfileName|x|x||||| msgVpnName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.0.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The ACL Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="aclProfileName">The name of the ACL Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnAclProfileResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnAclProfileResponse>> UpdateMsgVpnAclProfileAsyncWithHttpInfo (MsgVpnAclProfile body, string msgVpnName, string aclProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling AclProfileApi->UpdateMsgVpnAclProfile");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling AclProfileApi->UpdateMsgVpnAclProfile");
            // verify the required parameter 'aclProfileName' is set
            if (aclProfileName == null)
                throw new ApiException(400, "Missing required parameter 'aclProfileName' when calling AclProfileApi->UpdateMsgVpnAclProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/aclProfiles/{aclProfileName}";
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

            if (msgVpnName != null) localVarPathParams.Add("msgVpnName", this.Configuration.ApiClient.ParameterToString(msgVpnName)); // path parameter
            if (aclProfileName != null) localVarPathParams.Add("aclProfileName", this.Configuration.ApiClient.ParameterToString(aclProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("UpdateMsgVpnAclProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnAclProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnAclProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnAclProfileResponse)));
        }

    }
}
