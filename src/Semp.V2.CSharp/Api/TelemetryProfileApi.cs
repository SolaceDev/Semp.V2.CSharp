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
        public interface ITelemetryProfileApi : IApiAccessor
    {
        #region Synchronous Operations
        /// <summary>
        /// Create a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Create a Telemetry Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileResponse</returns>
        MsgVpnTelemetryProfileResponse CreateMsgVpnTelemetryProfile (MsgVpnTelemetryProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Create a Telemetry Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileResponse> CreateMsgVpnTelemetryProfileWithHttpInfo (MsgVpnTelemetryProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Receiver ACL Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Receiver ACL Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| receiverAclConnectExceptionAddress|x|x|||| telemetryProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Receiver ACL Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse</returns>
        MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse CreateMsgVpnTelemetryProfileReceiverAclConnectException (MsgVpnTelemetryProfileReceiverAclConnectException body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Receiver ACL Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Receiver ACL Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| receiverAclConnectExceptionAddress|x|x|||| telemetryProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Receiver ACL Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse> CreateMsgVpnTelemetryProfileReceiverAclConnectExceptionWithHttpInfo (MsgVpnTelemetryProfileReceiverAclConnectException body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Create a Trace Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x||x||| traceFilterName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterResponse</returns>
        MsgVpnTelemetryProfileTraceFilterResponse CreateMsgVpnTelemetryProfileTraceFilter (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Create a Trace Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x||x||| traceFilterName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse> CreateMsgVpnTelemetryProfileTraceFilterWithHttpInfo (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Telemetry Trace Filter Subscription object.
        /// </summary>
        /// <remarks>
        /// Create a Telemetry Trace Filter Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| subscription|x|x|||| subscriptionSyntax|x|x|||| telemetryProfileName|x||x||| traceFilterName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Trace Filter Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterSubscriptionResponse</returns>
        MsgVpnTelemetryProfileTraceFilterSubscriptionResponse CreateMsgVpnTelemetryProfileTraceFilterSubscription (MsgVpnTelemetryProfileTraceFilterSubscription body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Telemetry Trace Filter Subscription object.
        /// </summary>
        /// <remarks>
        /// Create a Telemetry Trace Filter Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| subscription|x|x|||| subscriptionSyntax|x|x|||| telemetryProfileName|x||x||| traceFilterName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Trace Filter Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterSubscriptionResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse> CreateMsgVpnTelemetryProfileTraceFilterSubscriptionWithHttpInfo (MsgVpnTelemetryProfileTraceFilterSubscription body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Delete a Telemetry Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnTelemetryProfile (string msgVpnName, string telemetryProfileName);

        /// <summary>
        /// Delete a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Delete a Telemetry Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnTelemetryProfileWithHttpInfo (string msgVpnName, string telemetryProfileName);
        /// <summary>
        /// Delete a Receiver ACL Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Receiver ACL Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnTelemetryProfileReceiverAclConnectException (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress);

        /// <summary>
        /// Delete a Receiver ACL Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Receiver ACL Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnTelemetryProfileReceiverAclConnectExceptionWithHttpInfo (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress);
        /// <summary>
        /// Delete a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Delete a Trace Filter object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnTelemetryProfileTraceFilter (string msgVpnName, string telemetryProfileName, string traceFilterName);

        /// <summary>
        /// Delete a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Delete a Trace Filter object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnTelemetryProfileTraceFilterWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName);
        /// <summary>
        /// Delete a Telemetry Trace Filter Subscription object.
        /// </summary>
        /// <remarks>
        /// Delete a Telemetry Trace Filter Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        SempMetaOnlyResponse DeleteMsgVpnTelemetryProfileTraceFilterSubscription (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax);

        /// <summary>
        /// Delete a Telemetry Trace Filter Subscription object.
        /// </summary>
        /// <remarks>
        /// Delete a Telemetry Trace Filter Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        ApiResponse<SempMetaOnlyResponse> DeleteMsgVpnTelemetryProfileTraceFilterSubscriptionWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax);
        /// <summary>
        /// Get a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Get a Telemetry Profile object.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileResponse</returns>
        MsgVpnTelemetryProfileResponse GetMsgVpnTelemetryProfile (string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Get a Telemetry Profile object.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileResponse> GetMsgVpnTelemetryProfileWithHttpInfo (string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Receiver ACL Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Receiver ACL Connect Exception object.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse</returns>
        MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse GetMsgVpnTelemetryProfileReceiverAclConnectException (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Receiver ACL Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Receiver ACL Connect Exception object.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse> GetMsgVpnTelemetryProfileReceiverAclConnectExceptionWithHttpInfo (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Receiver ACL Connect Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Receiver ACL Connect Exception objects.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse</returns>
        MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse GetMsgVpnTelemetryProfileReceiverAclConnectExceptions (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Receiver ACL Connect Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Receiver ACL Connect Exception objects.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse> GetMsgVpnTelemetryProfileReceiverAclConnectExceptionsWithHttpInfo (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Get a Trace Filter object.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterResponse</returns>
        MsgVpnTelemetryProfileTraceFilterResponse GetMsgVpnTelemetryProfileTraceFilter (string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Get a Trace Filter object.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse> GetMsgVpnTelemetryProfileTraceFilterWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Telemetry Trace Filter Subscription object.
        /// </summary>
        /// <remarks>
        /// Get a Telemetry Trace Filter Subscription object.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterSubscriptionResponse</returns>
        MsgVpnTelemetryProfileTraceFilterSubscriptionResponse GetMsgVpnTelemetryProfileTraceFilterSubscription (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Telemetry Trace Filter Subscription object.
        /// </summary>
        /// <remarks>
        /// Get a Telemetry Trace Filter Subscription object.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterSubscriptionResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse> GetMsgVpnTelemetryProfileTraceFilterSubscriptionWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Telemetry Trace Filter Subscription objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Telemetry Trace Filter Subscription objects.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse</returns>
        MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse GetMsgVpnTelemetryProfileTraceFilterSubscriptions (string msgVpnName, string telemetryProfileName, string traceFilterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Telemetry Trace Filter Subscription objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Telemetry Trace Filter Subscription objects.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse> GetMsgVpnTelemetryProfileTraceFilterSubscriptionsWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Trace Filter objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Trace Filter objects.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFiltersResponse</returns>
        MsgVpnTelemetryProfileTraceFiltersResponse GetMsgVpnTelemetryProfileTraceFilters (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Trace Filter objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Trace Filter objects.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFiltersResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileTraceFiltersResponse> GetMsgVpnTelemetryProfileTraceFiltersWithHttpInfo (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Telemetry Profile objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Telemetry Profile objects.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfilesResponse</returns>
        MsgVpnTelemetryProfilesResponse GetMsgVpnTelemetryProfiles (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Telemetry Profile objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Telemetry Profile objects.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfilesResponse</returns>
        ApiResponse<MsgVpnTelemetryProfilesResponse> GetMsgVpnTelemetryProfilesWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Replace a Telemetry Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileResponse</returns>
        MsgVpnTelemetryProfileResponse ReplaceMsgVpnTelemetryProfile (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Replace a Telemetry Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileResponse> ReplaceMsgVpnTelemetryProfileWithHttpInfo (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Replace a Trace Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x||||| traceFilterName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterResponse</returns>
        MsgVpnTelemetryProfileTraceFilterResponse ReplaceMsgVpnTelemetryProfileTraceFilter (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Replace a Trace Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x||||| traceFilterName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse> ReplaceMsgVpnTelemetryProfileTraceFilterWithHttpInfo (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Update a Telemetry Profile object. Any attribute missing from the request will be left unchanged.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileResponse</returns>
        MsgVpnTelemetryProfileResponse UpdateMsgVpnTelemetryProfile (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Update a Telemetry Profile object. Any attribute missing from the request will be left unchanged.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileResponse> UpdateMsgVpnTelemetryProfileWithHttpInfo (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Update a Trace Filter object. Any attribute missing from the request will be left unchanged.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x||||| traceFilterName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterResponse</returns>
        MsgVpnTelemetryProfileTraceFilterResponse UpdateMsgVpnTelemetryProfileTraceFilter (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Update a Trace Filter object. Any attribute missing from the request will be left unchanged.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x||||| traceFilterName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse> UpdateMsgVpnTelemetryProfileTraceFilterWithHttpInfo (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);
        #endregion Synchronous Operations
        #region Asynchronous Operations
        /// <summary>
        /// Create a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Create a Telemetry Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileResponse> CreateMsgVpnTelemetryProfileAsync (MsgVpnTelemetryProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Create a Telemetry Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileResponse>> CreateMsgVpnTelemetryProfileAsyncWithHttpInfo (MsgVpnTelemetryProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Receiver ACL Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Receiver ACL Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| receiverAclConnectExceptionAddress|x|x|||| telemetryProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Receiver ACL Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse> CreateMsgVpnTelemetryProfileReceiverAclConnectExceptionAsync (MsgVpnTelemetryProfileReceiverAclConnectException body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Receiver ACL Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Create a Receiver ACL Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| receiverAclConnectExceptionAddress|x|x|||| telemetryProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Receiver ACL Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse>> CreateMsgVpnTelemetryProfileReceiverAclConnectExceptionAsyncWithHttpInfo (MsgVpnTelemetryProfileReceiverAclConnectException body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Create a Trace Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x||x||| traceFilterName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterResponse> CreateMsgVpnTelemetryProfileTraceFilterAsync (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Create a Trace Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x||x||| traceFilterName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>> CreateMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Create a Telemetry Trace Filter Subscription object.
        /// </summary>
        /// <remarks>
        /// Create a Telemetry Trace Filter Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| subscription|x|x|||| subscriptionSyntax|x|x|||| telemetryProfileName|x||x||| traceFilterName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Trace Filter Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterSubscriptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse> CreateMsgVpnTelemetryProfileTraceFilterSubscriptionAsync (MsgVpnTelemetryProfileTraceFilterSubscription body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Create a Telemetry Trace Filter Subscription object.
        /// </summary>
        /// <remarks>
        /// Create a Telemetry Trace Filter Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| subscription|x|x|||| subscriptionSyntax|x|x|||| telemetryProfileName|x||x||| traceFilterName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Trace Filter Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterSubscriptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse>> CreateMsgVpnTelemetryProfileTraceFilterSubscriptionAsyncWithHttpInfo (MsgVpnTelemetryProfileTraceFilterSubscription body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Delete a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Delete a Telemetry Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnTelemetryProfileAsync (string msgVpnName, string telemetryProfileName);

        /// <summary>
        /// Delete a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Delete a Telemetry Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnTelemetryProfileAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName);
        /// <summary>
        /// Delete a Receiver ACL Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Receiver ACL Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnTelemetryProfileReceiverAclConnectExceptionAsync (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress);

        /// <summary>
        /// Delete a Receiver ACL Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Delete a Receiver ACL Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnTelemetryProfileReceiverAclConnectExceptionAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress);
        /// <summary>
        /// Delete a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Delete a Trace Filter object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnTelemetryProfileTraceFilterAsync (string msgVpnName, string telemetryProfileName, string traceFilterName);

        /// <summary>
        /// Delete a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Delete a Trace Filter object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName);
        /// <summary>
        /// Delete a Telemetry Trace Filter Subscription object.
        /// </summary>
        /// <remarks>
        /// Delete a Telemetry Trace Filter Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnTelemetryProfileTraceFilterSubscriptionAsync (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax);

        /// <summary>
        /// Delete a Telemetry Trace Filter Subscription object.
        /// </summary>
        /// <remarks>
        /// Delete a Telemetry Trace Filter Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnTelemetryProfileTraceFilterSubscriptionAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax);
        /// <summary>
        /// Get a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Get a Telemetry Profile object.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileResponse> GetMsgVpnTelemetryProfileAsync (string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Get a Telemetry Profile object.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileResponse>> GetMsgVpnTelemetryProfileAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Receiver ACL Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Receiver ACL Connect Exception object.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse> GetMsgVpnTelemetryProfileReceiverAclConnectExceptionAsync (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Receiver ACL Connect Exception object.
        /// </summary>
        /// <remarks>
        /// Get a Receiver ACL Connect Exception object.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse>> GetMsgVpnTelemetryProfileReceiverAclConnectExceptionAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Receiver ACL Connect Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Receiver ACL Connect Exception objects.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse> GetMsgVpnTelemetryProfileReceiverAclConnectExceptionsAsync (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Receiver ACL Connect Exception objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Receiver ACL Connect Exception objects.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse>> GetMsgVpnTelemetryProfileReceiverAclConnectExceptionsAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Get a Trace Filter object.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterResponse> GetMsgVpnTelemetryProfileTraceFilterAsync (string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Get a Trace Filter object.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>> GetMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a Telemetry Trace Filter Subscription object.
        /// </summary>
        /// <remarks>
        /// Get a Telemetry Trace Filter Subscription object.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterSubscriptionResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse> GetMsgVpnTelemetryProfileTraceFilterSubscriptionAsync (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Get a Telemetry Trace Filter Subscription object.
        /// </summary>
        /// <remarks>
        /// Get a Telemetry Trace Filter Subscription object.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterSubscriptionResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse>> GetMsgVpnTelemetryProfileTraceFilterSubscriptionAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Get a list of Telemetry Trace Filter Subscription objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Telemetry Trace Filter Subscription objects.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse> GetMsgVpnTelemetryProfileTraceFilterSubscriptionsAsync (string msgVpnName, string telemetryProfileName, string traceFilterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Telemetry Trace Filter Subscription objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Telemetry Trace Filter Subscription objects.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse>> GetMsgVpnTelemetryProfileTraceFilterSubscriptionsAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Trace Filter objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Trace Filter objects.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFiltersResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFiltersResponse> GetMsgVpnTelemetryProfileTraceFiltersAsync (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Trace Filter objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Trace Filter objects.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFiltersResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFiltersResponse>> GetMsgVpnTelemetryProfileTraceFiltersAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Get a list of Telemetry Profile objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Telemetry Profile objects.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfilesResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfilesResponse> GetMsgVpnTelemetryProfilesAsync (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);

        /// <summary>
        /// Get a list of Telemetry Profile objects.
        /// </summary>
        /// <remarks>
        /// Get a list of Telemetry Profile objects.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfilesResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfilesResponse>> GetMsgVpnTelemetryProfilesAsyncWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null);
        /// <summary>
        /// Replace a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Replace a Telemetry Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileResponse> ReplaceMsgVpnTelemetryProfileAsync (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Replace a Telemetry Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileResponse>> ReplaceMsgVpnTelemetryProfileAsyncWithHttpInfo (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Replace a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Replace a Trace Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x||||| traceFilterName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterResponse> ReplaceMsgVpnTelemetryProfileTraceFilterAsync (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Replace a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Replace a Trace Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x||||| traceFilterName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>> ReplaceMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Update a Telemetry Profile object. Any attribute missing from the request will be left unchanged.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileResponse> UpdateMsgVpnTelemetryProfileAsync (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Telemetry Profile object.
        /// </summary>
        /// <remarks>
        /// Update a Telemetry Profile object. Any attribute missing from the request will be left unchanged.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileResponse>> UpdateMsgVpnTelemetryProfileAsyncWithHttpInfo (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null);
        /// <summary>
        /// Update a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Update a Trace Filter object. Any attribute missing from the request will be left unchanged.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x||||| traceFilterName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterResponse> UpdateMsgVpnTelemetryProfileTraceFilterAsync (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);

        /// <summary>
        /// Update a Trace Filter object.
        /// </summary>
        /// <remarks>
        /// Update a Trace Filter object. Any attribute missing from the request will be left unchanged.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x||||| traceFilterName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </remarks>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterResponse)</returns>
        System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>> UpdateMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null);
        #endregion Asynchronous Operations
    }

    /// <summary>
    /// Represents a collection of functions to interact with the API endpoints
    /// </summary>
        public partial class TelemetryProfileApi : ITelemetryProfileApi
    {
        private Semp.V2.CSharp.Client.ExceptionFactory _exceptionFactory = (name, response) => null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryProfileApi"/> class.
        /// </summary>
        /// <returns></returns>
        public TelemetryProfileApi(String basePath)
        {
            this.Configuration = new Semp.V2.CSharp.Client.Configuration { BasePath = basePath };

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryProfileApi"/> class
        /// </summary>
        /// <returns></returns>
        public TelemetryProfileApi()
        {
            this.Configuration = Semp.V2.CSharp.Client.Configuration.Default;

            ExceptionFactory = Semp.V2.CSharp.Client.Configuration.DefaultExceptionFactory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TelemetryProfileApi"/> class
        /// using Configuration object
        /// </summary>
        /// <param name="configuration">An instance of Configuration</param>
        /// <returns></returns>
        public TelemetryProfileApi(Semp.V2.CSharp.Client.Configuration configuration = null)
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
        /// Create a Telemetry Profile object. Create a Telemetry Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileResponse</returns>
        public MsgVpnTelemetryProfileResponse CreateMsgVpnTelemetryProfile (MsgVpnTelemetryProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileResponse> localVarResponse = CreateMsgVpnTelemetryProfileWithHttpInfo(body, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Telemetry Profile object. Create a Telemetry Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileResponse > CreateMsgVpnTelemetryProfileWithHttpInfo (MsgVpnTelemetryProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfile");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles";
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
                Exception exception = ExceptionFactory("CreateMsgVpnTelemetryProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileResponse)));
        }

        /// <summary>
        /// Create a Telemetry Profile object. Create a Telemetry Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileResponse> CreateMsgVpnTelemetryProfileAsync (MsgVpnTelemetryProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileResponse> localVarResponse = await CreateMsgVpnTelemetryProfileAsyncWithHttpInfo(body, msgVpnName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Telemetry Profile object. Create a Telemetry Profile object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x|x||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileResponse>> CreateMsgVpnTelemetryProfileAsyncWithHttpInfo (MsgVpnTelemetryProfile body, string msgVpnName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfile");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles";
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
                Exception exception = ExceptionFactory("CreateMsgVpnTelemetryProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileResponse)));
        }

        /// <summary>
        /// Create a Receiver ACL Connect Exception object. Create a Receiver ACL Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| receiverAclConnectExceptionAddress|x|x|||| telemetryProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Receiver ACL Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse</returns>
        public MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse CreateMsgVpnTelemetryProfileReceiverAclConnectException (MsgVpnTelemetryProfileReceiverAclConnectException body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse> localVarResponse = CreateMsgVpnTelemetryProfileReceiverAclConnectExceptionWithHttpInfo(body, msgVpnName, telemetryProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Receiver ACL Connect Exception object. Create a Receiver ACL Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| receiverAclConnectExceptionAddress|x|x|||| telemetryProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Receiver ACL Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse > CreateMsgVpnTelemetryProfileReceiverAclConnectExceptionWithHttpInfo (MsgVpnTelemetryProfileReceiverAclConnectException body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileReceiverAclConnectException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileReceiverAclConnectException");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileReceiverAclConnectException");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnTelemetryProfileReceiverAclConnectException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse)));
        }

        /// <summary>
        /// Create a Receiver ACL Connect Exception object. Create a Receiver ACL Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| receiverAclConnectExceptionAddress|x|x|||| telemetryProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Receiver ACL Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse> CreateMsgVpnTelemetryProfileReceiverAclConnectExceptionAsync (MsgVpnTelemetryProfileReceiverAclConnectException body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse> localVarResponse = await CreateMsgVpnTelemetryProfileReceiverAclConnectExceptionAsyncWithHttpInfo(body, msgVpnName, telemetryProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Receiver ACL Connect Exception object. Create a Receiver ACL Connect Exception object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| receiverAclConnectExceptionAddress|x|x|||| telemetryProfileName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Receiver ACL Connect Exception object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse>> CreateMsgVpnTelemetryProfileReceiverAclConnectExceptionAsyncWithHttpInfo (MsgVpnTelemetryProfileReceiverAclConnectException body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileReceiverAclConnectException");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileReceiverAclConnectException");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileReceiverAclConnectException");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnTelemetryProfileReceiverAclConnectException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse)));
        }

        /// <summary>
        /// Create a Trace Filter object. Create a Trace Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x||x||| traceFilterName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterResponse</returns>
        public MsgVpnTelemetryProfileTraceFilterResponse CreateMsgVpnTelemetryProfileTraceFilter (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse> localVarResponse = CreateMsgVpnTelemetryProfileTraceFilterWithHttpInfo(body, msgVpnName, telemetryProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Trace Filter object. Create a Trace Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x||x||| traceFilterName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileTraceFilterResponse > CreateMsgVpnTelemetryProfileTraceFilterWithHttpInfo (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilter");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnTelemetryProfileTraceFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterResponse)));
        }

        /// <summary>
        /// Create a Trace Filter object. Create a Trace Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x||x||| traceFilterName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterResponse> CreateMsgVpnTelemetryProfileTraceFilterAsync (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse> localVarResponse = await CreateMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo(body, msgVpnName, telemetryProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Trace Filter object. Create a Trace Filter object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| telemetryProfileName|x||x||| traceFilterName|x|x||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>> CreateMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilter");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnTelemetryProfileTraceFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterResponse)));
        }

        /// <summary>
        /// Create a Telemetry Trace Filter Subscription object. Create a Telemetry Trace Filter Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| subscription|x|x|||| subscriptionSyntax|x|x|||| telemetryProfileName|x||x||| traceFilterName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Trace Filter Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterSubscriptionResponse</returns>
        public MsgVpnTelemetryProfileTraceFilterSubscriptionResponse CreateMsgVpnTelemetryProfileTraceFilterSubscription (MsgVpnTelemetryProfileTraceFilterSubscription body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse> localVarResponse = CreateMsgVpnTelemetryProfileTraceFilterSubscriptionWithHttpInfo(body, msgVpnName, telemetryProfileName, traceFilterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Create a Telemetry Trace Filter Subscription object. Create a Telemetry Trace Filter Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| subscription|x|x|||| subscriptionSyntax|x|x|||| telemetryProfileName|x||x||| traceFilterName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Trace Filter Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterSubscriptionResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileTraceFilterSubscriptionResponse > CreateMsgVpnTelemetryProfileTraceFilterSubscriptionWithHttpInfo (MsgVpnTelemetryProfileTraceFilterSubscription body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilterSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnTelemetryProfileTraceFilterSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterSubscriptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterSubscriptionResponse)));
        }

        /// <summary>
        /// Create a Telemetry Trace Filter Subscription object. Create a Telemetry Trace Filter Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| subscription|x|x|||| subscriptionSyntax|x|x|||| telemetryProfileName|x||x||| traceFilterName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Trace Filter Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterSubscriptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse> CreateMsgVpnTelemetryProfileTraceFilterSubscriptionAsync (MsgVpnTelemetryProfileTraceFilterSubscription body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse> localVarResponse = await CreateMsgVpnTelemetryProfileTraceFilterSubscriptionAsyncWithHttpInfo(body, msgVpnName, telemetryProfileName, traceFilterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Create a Telemetry Trace Filter Subscription object. Create a Telemetry Trace Filter Subscription object. Any attribute missing from the request will be set to its default value. The creation of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Required|Read-Only|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --:|:- --:|:- --: msgVpnName|x||x||| subscription|x|x|||| subscriptionSyntax|x|x|||| telemetryProfileName|x||x||| traceFilterName|x||x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Trace Filter Subscription object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterSubscriptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse>> CreateMsgVpnTelemetryProfileTraceFilterSubscriptionAsyncWithHttpInfo (MsgVpnTelemetryProfileTraceFilterSubscription body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->CreateMsgVpnTelemetryProfileTraceFilterSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
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
                Exception exception = ExceptionFactory("CreateMsgVpnTelemetryProfileTraceFilterSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterSubscriptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterSubscriptionResponse)));
        }

        /// <summary>
        /// Delete a Telemetry Profile object. Delete a Telemetry Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnTelemetryProfile (string msgVpnName, string telemetryProfileName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnTelemetryProfileWithHttpInfo(msgVpnName, telemetryProfileName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Telemetry Profile object. Delete a Telemetry Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnTelemetryProfileWithHttpInfo (string msgVpnName, string telemetryProfileName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfile");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnTelemetryProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Telemetry Profile object. Delete a Telemetry Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnTelemetryProfileAsync (string msgVpnName, string telemetryProfileName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnTelemetryProfileAsyncWithHttpInfo(msgVpnName, telemetryProfileName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Telemetry Profile object. Delete a Telemetry Profile object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnTelemetryProfileAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfile");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnTelemetryProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Receiver ACL Connect Exception object. Delete a Receiver ACL Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnTelemetryProfileReceiverAclConnectException (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnTelemetryProfileReceiverAclConnectExceptionWithHttpInfo(msgVpnName, telemetryProfileName, receiverAclConnectExceptionAddress);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Receiver ACL Connect Exception object. Delete a Receiver ACL Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnTelemetryProfileReceiverAclConnectExceptionWithHttpInfo (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileReceiverAclConnectException");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileReceiverAclConnectException");
            // verify the required parameter 'receiverAclConnectExceptionAddress' is set
            if (receiverAclConnectExceptionAddress == null)
                throw new ApiException(400, "Missing required parameter 'receiverAclConnectExceptionAddress' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileReceiverAclConnectException");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions/{receiverAclConnectExceptionAddress}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (receiverAclConnectExceptionAddress != null) localVarPathParams.Add("receiverAclConnectExceptionAddress", this.Configuration.ApiClient.ParameterToString(receiverAclConnectExceptionAddress)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnTelemetryProfileReceiverAclConnectException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Receiver ACL Connect Exception object. Delete a Receiver ACL Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnTelemetryProfileReceiverAclConnectExceptionAsync (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnTelemetryProfileReceiverAclConnectExceptionAsyncWithHttpInfo(msgVpnName, telemetryProfileName, receiverAclConnectExceptionAddress);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Receiver ACL Connect Exception object. Delete a Receiver ACL Connect Exception object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnTelemetryProfileReceiverAclConnectExceptionAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileReceiverAclConnectException");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileReceiverAclConnectException");
            // verify the required parameter 'receiverAclConnectExceptionAddress' is set
            if (receiverAclConnectExceptionAddress == null)
                throw new ApiException(400, "Missing required parameter 'receiverAclConnectExceptionAddress' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileReceiverAclConnectException");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions/{receiverAclConnectExceptionAddress}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (receiverAclConnectExceptionAddress != null) localVarPathParams.Add("receiverAclConnectExceptionAddress", this.Configuration.ApiClient.ParameterToString(receiverAclConnectExceptionAddress)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnTelemetryProfileReceiverAclConnectException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Trace Filter object. Delete a Trace Filter object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnTelemetryProfileTraceFilter (string msgVpnName, string telemetryProfileName, string traceFilterName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnTelemetryProfileTraceFilterWithHttpInfo(msgVpnName, telemetryProfileName, traceFilterName);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Trace Filter object. Delete a Trace Filter object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnTelemetryProfileTraceFilterWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilter");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnTelemetryProfileTraceFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Trace Filter object. Delete a Trace Filter object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnTelemetryProfileTraceFilterAsync (string msgVpnName, string telemetryProfileName, string traceFilterName)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo(msgVpnName, telemetryProfileName, traceFilterName);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Trace Filter object. Delete a Trace Filter object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilter");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnTelemetryProfileTraceFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Telemetry Trace Filter Subscription object. Delete a Telemetry Trace Filter Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <returns>SempMetaOnlyResponse</returns>
        public SempMetaOnlyResponse DeleteMsgVpnTelemetryProfileTraceFilterSubscription (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = DeleteMsgVpnTelemetryProfileTraceFilterSubscriptionWithHttpInfo(msgVpnName, telemetryProfileName, traceFilterName, subscription, subscriptionSyntax);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Delete a Telemetry Trace Filter Subscription object. Delete a Telemetry Trace Filter Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <returns>ApiResponse of SempMetaOnlyResponse</returns>
        public ApiResponse< SempMetaOnlyResponse > DeleteMsgVpnTelemetryProfileTraceFilterSubscriptionWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'subscription' is set
            if (subscription == null)
                throw new ApiException(400, "Missing required parameter 'subscription' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'subscriptionSyntax' is set
            if (subscriptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'subscriptionSyntax' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilterSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions/{subscription},{subscriptionSyntax}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
            if (subscription != null) localVarPathParams.Add("subscription", this.Configuration.ApiClient.ParameterToString(subscription)); // path parameter
            if (subscriptionSyntax != null) localVarPathParams.Add("subscriptionSyntax", this.Configuration.ApiClient.ParameterToString(subscriptionSyntax)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnTelemetryProfileTraceFilterSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Delete a Telemetry Trace Filter Subscription object. Delete a Telemetry Trace Filter Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <returns>Task of SempMetaOnlyResponse</returns>
        public async System.Threading.Tasks.Task<SempMetaOnlyResponse> DeleteMsgVpnTelemetryProfileTraceFilterSubscriptionAsync (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax)
        {
             ApiResponse<SempMetaOnlyResponse> localVarResponse = await DeleteMsgVpnTelemetryProfileTraceFilterSubscriptionAsyncWithHttpInfo(msgVpnName, telemetryProfileName, traceFilterName, subscription, subscriptionSyntax);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Delete a Telemetry Trace Filter Subscription object. Delete a Telemetry Trace Filter Subscription object. The deletion of instances of this object are synchronized to HA mates and replication sites via config-sync.  Trace filter subscriptions control which messages will be attracted by the tracing filter.  A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <returns>Task of ApiResponse (SempMetaOnlyResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<SempMetaOnlyResponse>> DeleteMsgVpnTelemetryProfileTraceFilterSubscriptionAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'subscription' is set
            if (subscription == null)
                throw new ApiException(400, "Missing required parameter 'subscription' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'subscriptionSyntax' is set
            if (subscriptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'subscriptionSyntax' when calling TelemetryProfileApi->DeleteMsgVpnTelemetryProfileTraceFilterSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions/{subscription},{subscriptionSyntax}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
            if (subscription != null) localVarPathParams.Add("subscription", this.Configuration.ApiClient.ParameterToString(subscription)); // path parameter
            if (subscriptionSyntax != null) localVarPathParams.Add("subscriptionSyntax", this.Configuration.ApiClient.ParameterToString(subscriptionSyntax)); // path parameter
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
                Exception exception = ExceptionFactory("DeleteMsgVpnTelemetryProfileTraceFilterSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<SempMetaOnlyResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (SempMetaOnlyResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(SempMetaOnlyResponse)));
        }

        /// <summary>
        /// Get a Telemetry Profile object. Get a Telemetry Profile object.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileResponse</returns>
        public MsgVpnTelemetryProfileResponse GetMsgVpnTelemetryProfile (string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileResponse> localVarResponse = GetMsgVpnTelemetryProfileWithHttpInfo(msgVpnName, telemetryProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Telemetry Profile object. Get a Telemetry Profile object.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileResponse > GetMsgVpnTelemetryProfileWithHttpInfo (string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfile");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileResponse)));
        }

        /// <summary>
        /// Get a Telemetry Profile object. Get a Telemetry Profile object.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileResponse> GetMsgVpnTelemetryProfileAsync (string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileResponse> localVarResponse = await GetMsgVpnTelemetryProfileAsyncWithHttpInfo(msgVpnName, telemetryProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Telemetry Profile object. Get a Telemetry Profile object.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileResponse>> GetMsgVpnTelemetryProfileAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfile");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileResponse)));
        }

        /// <summary>
        /// Get a Receiver ACL Connect Exception object. Get a Receiver ACL Connect Exception object.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse</returns>
        public MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse GetMsgVpnTelemetryProfileReceiverAclConnectException (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse> localVarResponse = GetMsgVpnTelemetryProfileReceiverAclConnectExceptionWithHttpInfo(msgVpnName, telemetryProfileName, receiverAclConnectExceptionAddress, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Receiver ACL Connect Exception object. Get a Receiver ACL Connect Exception object.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse > GetMsgVpnTelemetryProfileReceiverAclConnectExceptionWithHttpInfo (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileReceiverAclConnectException");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileReceiverAclConnectException");
            // verify the required parameter 'receiverAclConnectExceptionAddress' is set
            if (receiverAclConnectExceptionAddress == null)
                throw new ApiException(400, "Missing required parameter 'receiverAclConnectExceptionAddress' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileReceiverAclConnectException");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions/{receiverAclConnectExceptionAddress}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (receiverAclConnectExceptionAddress != null) localVarPathParams.Add("receiverAclConnectExceptionAddress", this.Configuration.ApiClient.ParameterToString(receiverAclConnectExceptionAddress)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfileReceiverAclConnectException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse)));
        }

        /// <summary>
        /// Get a Receiver ACL Connect Exception object. Get a Receiver ACL Connect Exception object.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse> GetMsgVpnTelemetryProfileReceiverAclConnectExceptionAsync (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse> localVarResponse = await GetMsgVpnTelemetryProfileReceiverAclConnectExceptionAsyncWithHttpInfo(msgVpnName, telemetryProfileName, receiverAclConnectExceptionAddress, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Receiver ACL Connect Exception object. Get a Receiver ACL Connect Exception object.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="receiverAclConnectExceptionAddress">The IP address/netmask of the receiver connect exception in CIDR form.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse>> GetMsgVpnTelemetryProfileReceiverAclConnectExceptionAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string receiverAclConnectExceptionAddress, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileReceiverAclConnectException");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileReceiverAclConnectException");
            // verify the required parameter 'receiverAclConnectExceptionAddress' is set
            if (receiverAclConnectExceptionAddress == null)
                throw new ApiException(400, "Missing required parameter 'receiverAclConnectExceptionAddress' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileReceiverAclConnectException");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions/{receiverAclConnectExceptionAddress}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (receiverAclConnectExceptionAddress != null) localVarPathParams.Add("receiverAclConnectExceptionAddress", this.Configuration.ApiClient.ParameterToString(receiverAclConnectExceptionAddress)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfileReceiverAclConnectException", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileReceiverAclConnectExceptionResponse)));
        }

        /// <summary>
        /// Get a list of Receiver ACL Connect Exception objects. Get a list of Receiver ACL Connect Exception objects.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse</returns>
        public MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse GetMsgVpnTelemetryProfileReceiverAclConnectExceptions (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse> localVarResponse = GetMsgVpnTelemetryProfileReceiverAclConnectExceptionsWithHttpInfo(msgVpnName, telemetryProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Receiver ACL Connect Exception objects. Get a list of Receiver ACL Connect Exception objects.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse > GetMsgVpnTelemetryProfileReceiverAclConnectExceptionsWithHttpInfo (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileReceiverAclConnectExceptions");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileReceiverAclConnectExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfileReceiverAclConnectExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse)));
        }

        /// <summary>
        /// Get a list of Receiver ACL Connect Exception objects. Get a list of Receiver ACL Connect Exception objects.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse> GetMsgVpnTelemetryProfileReceiverAclConnectExceptionsAsync (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse> localVarResponse = await GetMsgVpnTelemetryProfileReceiverAclConnectExceptionsAsyncWithHttpInfo(msgVpnName, telemetryProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Receiver ACL Connect Exception objects. Get a list of Receiver ACL Connect Exception objects.  A Receiver ACL Connect Exception is an exception to the default action to take when a receiver connects to the broker. Exceptions must be expressed as an IP address/netmask in CIDR form.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| receiverAclConnectExceptionAddress|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse>> GetMsgVpnTelemetryProfileReceiverAclConnectExceptionsAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileReceiverAclConnectExceptions");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileReceiverAclConnectExceptions");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/receiverAclConnectExceptions";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfileReceiverAclConnectExceptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileReceiverAclConnectExceptionsResponse)));
        }

        /// <summary>
        /// Get a Trace Filter object. Get a Trace Filter object.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterResponse</returns>
        public MsgVpnTelemetryProfileTraceFilterResponse GetMsgVpnTelemetryProfileTraceFilter (string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse> localVarResponse = GetMsgVpnTelemetryProfileTraceFilterWithHttpInfo(msgVpnName, telemetryProfileName, traceFilterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Trace Filter object. Get a Trace Filter object.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileTraceFilterResponse > GetMsgVpnTelemetryProfileTraceFilterWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilter");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfileTraceFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterResponse)));
        }

        /// <summary>
        /// Get a Trace Filter object. Get a Trace Filter object.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterResponse> GetMsgVpnTelemetryProfileTraceFilterAsync (string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse> localVarResponse = await GetMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo(msgVpnName, telemetryProfileName, traceFilterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Trace Filter object. Get a Trace Filter object.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>> GetMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilter");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfileTraceFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterResponse)));
        }

        /// <summary>
        /// Get a Telemetry Trace Filter Subscription object. Get a Telemetry Trace Filter Subscription object.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterSubscriptionResponse</returns>
        public MsgVpnTelemetryProfileTraceFilterSubscriptionResponse GetMsgVpnTelemetryProfileTraceFilterSubscription (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse> localVarResponse = GetMsgVpnTelemetryProfileTraceFilterSubscriptionWithHttpInfo(msgVpnName, telemetryProfileName, traceFilterName, subscription, subscriptionSyntax, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a Telemetry Trace Filter Subscription object. Get a Telemetry Trace Filter Subscription object.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterSubscriptionResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileTraceFilterSubscriptionResponse > GetMsgVpnTelemetryProfileTraceFilterSubscriptionWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'subscription' is set
            if (subscription == null)
                throw new ApiException(400, "Missing required parameter 'subscription' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'subscriptionSyntax' is set
            if (subscriptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'subscriptionSyntax' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions/{subscription},{subscriptionSyntax}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
            if (subscription != null) localVarPathParams.Add("subscription", this.Configuration.ApiClient.ParameterToString(subscription)); // path parameter
            if (subscriptionSyntax != null) localVarPathParams.Add("subscriptionSyntax", this.Configuration.ApiClient.ParameterToString(subscriptionSyntax)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfileTraceFilterSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterSubscriptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterSubscriptionResponse)));
        }

        /// <summary>
        /// Get a Telemetry Trace Filter Subscription object. Get a Telemetry Trace Filter Subscription object.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterSubscriptionResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse> GetMsgVpnTelemetryProfileTraceFilterSubscriptionAsync (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse> localVarResponse = await GetMsgVpnTelemetryProfileTraceFilterSubscriptionAsyncWithHttpInfo(msgVpnName, telemetryProfileName, traceFilterName, subscription, subscriptionSyntax, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a Telemetry Trace Filter Subscription object. Get a Telemetry Trace Filter Subscription object.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="subscription">Messages matching this subscription will follow this filter&#x27;s configuration.</param>
        /// <param name="subscriptionSyntax">The syntax of the trace filter subscription.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterSubscriptionResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse>> GetMsgVpnTelemetryProfileTraceFilterSubscriptionAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, string subscription, string subscriptionSyntax, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'subscription' is set
            if (subscription == null)
                throw new ApiException(400, "Missing required parameter 'subscription' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscription");
            // verify the required parameter 'subscriptionSyntax' is set
            if (subscriptionSyntax == null)
                throw new ApiException(400, "Missing required parameter 'subscriptionSyntax' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscription");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions/{subscription},{subscriptionSyntax}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
            if (subscription != null) localVarPathParams.Add("subscription", this.Configuration.ApiClient.ParameterToString(subscription)); // path parameter
            if (subscriptionSyntax != null) localVarPathParams.Add("subscriptionSyntax", this.Configuration.ApiClient.ParameterToString(subscriptionSyntax)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfileTraceFilterSubscription", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterSubscriptionResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterSubscriptionResponse)));
        }

        /// <summary>
        /// Get a list of Telemetry Trace Filter Subscription objects. Get a list of Telemetry Trace Filter Subscription objects.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse</returns>
        public MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse GetMsgVpnTelemetryProfileTraceFilterSubscriptions (string msgVpnName, string telemetryProfileName, string traceFilterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse> localVarResponse = GetMsgVpnTelemetryProfileTraceFilterSubscriptionsWithHttpInfo(msgVpnName, telemetryProfileName, traceFilterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Telemetry Trace Filter Subscription objects. Get a list of Telemetry Trace Filter Subscription objects.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse > GetMsgVpnTelemetryProfileTraceFilterSubscriptionsWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscriptions");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscriptions");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscriptions");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfileTraceFilterSubscriptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse)));
        }

        /// <summary>
        /// Get a list of Telemetry Trace Filter Subscription objects. Get a list of Telemetry Trace Filter Subscription objects.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse> GetMsgVpnTelemetryProfileTraceFilterSubscriptionsAsync (string msgVpnName, string telemetryProfileName, string traceFilterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse> localVarResponse = await GetMsgVpnTelemetryProfileTraceFilterSubscriptionsAsyncWithHttpInfo(msgVpnName, telemetryProfileName, traceFilterName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Telemetry Trace Filter Subscription objects. Get a list of Telemetry Trace Filter Subscription objects.  Trace filter subscriptions control which messages will be attracted by the tracing filter.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| subscription|x||| subscriptionSyntax|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse>> GetMsgVpnTelemetryProfileTraceFilterSubscriptionsAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, string traceFilterName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscriptions");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscriptions");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilterSubscriptions");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}/subscriptions";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfileTraceFilterSubscriptions", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterSubscriptionsResponse)));
        }

        /// <summary>
        /// Get a list of Trace Filter objects. Get a list of Trace Filter objects.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFiltersResponse</returns>
        public MsgVpnTelemetryProfileTraceFiltersResponse GetMsgVpnTelemetryProfileTraceFilters (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFiltersResponse> localVarResponse = GetMsgVpnTelemetryProfileTraceFiltersWithHttpInfo(msgVpnName, telemetryProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Trace Filter objects. Get a list of Trace Filter objects.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFiltersResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileTraceFiltersResponse > GetMsgVpnTelemetryProfileTraceFiltersWithHttpInfo (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilters");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilters");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfileTraceFilters", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFiltersResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFiltersResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFiltersResponse)));
        }

        /// <summary>
        /// Get a list of Trace Filter objects. Get a list of Trace Filter objects.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFiltersResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFiltersResponse> GetMsgVpnTelemetryProfileTraceFiltersAsync (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFiltersResponse> localVarResponse = await GetMsgVpnTelemetryProfileTraceFiltersAsyncWithHttpInfo(msgVpnName, telemetryProfileName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Trace Filter objects. Get a list of Trace Filter objects.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x||| traceFilterName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFiltersResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFiltersResponse>> GetMsgVpnTelemetryProfileTraceFiltersAsyncWithHttpInfo (string msgVpnName, string telemetryProfileName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilters");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfileTraceFilters");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfileTraceFilters", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFiltersResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFiltersResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFiltersResponse)));
        }

        /// <summary>
        /// Get a list of Telemetry Profile objects. Get a list of Telemetry Profile objects.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfilesResponse</returns>
        public MsgVpnTelemetryProfilesResponse GetMsgVpnTelemetryProfiles (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfilesResponse> localVarResponse = GetMsgVpnTelemetryProfilesWithHttpInfo(msgVpnName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Get a list of Telemetry Profile objects. Get a list of Telemetry Profile objects.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfilesResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfilesResponse > GetMsgVpnTelemetryProfilesWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfiles");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles";
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfiles", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfilesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfilesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfilesResponse)));
        }

        /// <summary>
        /// Get a list of Telemetry Profile objects. Get a list of Telemetry Profile objects.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfilesResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfilesResponse> GetMsgVpnTelemetryProfilesAsync (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfilesResponse> localVarResponse = await GetMsgVpnTelemetryProfilesAsyncWithHttpInfo(msgVpnName, count, cursor, opaquePassword, where, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Get a list of Telemetry Profile objects. Get a list of Telemetry Profile objects.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Write-Only|Deprecated|Opaque :- --|:- --:|:- --:|:- --:|:- --: msgVpnName|x||| telemetryProfileName|x|||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-only\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="count">Limit the count of objects in the response. See the documentation for the &#x60;count&#x60; parameter. (optional, default to 10)</param>
        /// <param name="cursor">The cursor, or position, for the next page of objects. See the documentation for the &#x60;cursor&#x60; parameter. (optional)</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="where">Include in the response only objects where certain conditions are true. See the the documentation for the &#x60;where&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfilesResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfilesResponse>> GetMsgVpnTelemetryProfilesAsyncWithHttpInfo (string msgVpnName, int? count = null, string cursor = null, string opaquePassword = null, List<string> where = null, List<string> select = null)
        {
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->GetMsgVpnTelemetryProfiles");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles";
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
                Exception exception = ExceptionFactory("GetMsgVpnTelemetryProfiles", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfilesResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfilesResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfilesResponse)));
        }

        /// <summary>
        /// Replace a Telemetry Profile object. Replace a Telemetry Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileResponse</returns>
        public MsgVpnTelemetryProfileResponse ReplaceMsgVpnTelemetryProfile (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileResponse> localVarResponse = ReplaceMsgVpnTelemetryProfileWithHttpInfo(body, msgVpnName, telemetryProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Telemetry Profile object. Replace a Telemetry Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileResponse > ReplaceMsgVpnTelemetryProfileWithHttpInfo (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfile");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfile");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("ReplaceMsgVpnTelemetryProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileResponse)));
        }

        /// <summary>
        /// Replace a Telemetry Profile object. Replace a Telemetry Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileResponse> ReplaceMsgVpnTelemetryProfileAsync (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileResponse> localVarResponse = await ReplaceMsgVpnTelemetryProfileAsyncWithHttpInfo(body, msgVpnName, telemetryProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Telemetry Profile object. Replace a Telemetry Profile object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileResponse>> ReplaceMsgVpnTelemetryProfileAsyncWithHttpInfo (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfile");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfile");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("ReplaceMsgVpnTelemetryProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileResponse)));
        }

        /// <summary>
        /// Replace a Trace Filter object. Replace a Trace Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x||||| traceFilterName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterResponse</returns>
        public MsgVpnTelemetryProfileTraceFilterResponse ReplaceMsgVpnTelemetryProfileTraceFilter (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse> localVarResponse = ReplaceMsgVpnTelemetryProfileTraceFilterWithHttpInfo(body, msgVpnName, telemetryProfileName, traceFilterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Replace a Trace Filter object. Replace a Trace Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x||||| traceFilterName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileTraceFilterResponse > ReplaceMsgVpnTelemetryProfileTraceFilterWithHttpInfo (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfileTraceFilter");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
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
                Exception exception = ExceptionFactory("ReplaceMsgVpnTelemetryProfileTraceFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterResponse)));
        }

        /// <summary>
        /// Replace a Trace Filter object. Replace a Trace Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x||||| traceFilterName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterResponse> ReplaceMsgVpnTelemetryProfileTraceFilterAsync (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse> localVarResponse = await ReplaceMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo(body, msgVpnName, telemetryProfileName, traceFilterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Replace a Trace Filter object. Replace a Trace Filter object. Any attribute missing from the request will be set to its default value, subject to the exceptions in note 4.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Const|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x||x||||| telemetryProfileName|x||x||||| traceFilterName|x||x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>> ReplaceMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->ReplaceMsgVpnTelemetryProfileTraceFilter");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
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
                Exception exception = ExceptionFactory("ReplaceMsgVpnTelemetryProfileTraceFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterResponse)));
        }

        /// <summary>
        /// Update a Telemetry Profile object. Update a Telemetry Profile object. Any attribute missing from the request will be left unchanged.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileResponse</returns>
        public MsgVpnTelemetryProfileResponse UpdateMsgVpnTelemetryProfile (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileResponse> localVarResponse = UpdateMsgVpnTelemetryProfileWithHttpInfo(body, msgVpnName, telemetryProfileName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Telemetry Profile object. Update a Telemetry Profile object. Any attribute missing from the request will be left unchanged.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileResponse > UpdateMsgVpnTelemetryProfileWithHttpInfo (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfile");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfile");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("UpdateMsgVpnTelemetryProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileResponse)));
        }

        /// <summary>
        /// Update a Telemetry Profile object. Update a Telemetry Profile object. Any attribute missing from the request will be left unchanged.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileResponse> UpdateMsgVpnTelemetryProfileAsync (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileResponse> localVarResponse = await UpdateMsgVpnTelemetryProfileAsyncWithHttpInfo(body, msgVpnName, telemetryProfileName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Telemetry Profile object. Update a Telemetry Profile object. Any attribute missing from the request will be left unchanged.  Using the Telemetry Profile allows trace spans to be generated as messages are processed by the broker. The generated spans are stored persistently on the broker and may be consumed by the Solace receiver component of an OpenTelemetry Collector.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x|||||    The following attributes in the request may only be provided in certain combinations with other attributes:   Class|Attribute|Requires|Conflicts :- --|:- --|:- --|:- -- EventThreshold|clearPercent|setPercent|clearValue, setValue EventThreshold|clearValue|setValue|clearPercent, setPercent EventThreshold|setPercent|clearPercent|clearValue, setValue EventThreshold|setValue|clearValue|clearPercent, setPercent    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Telemetry Profile object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileResponse>> UpdateMsgVpnTelemetryProfileAsyncWithHttpInfo (MsgVpnTelemetryProfile body, string msgVpnName, string telemetryProfileName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfile");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfile");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfile");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
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
                Exception exception = ExceptionFactory("UpdateMsgVpnTelemetryProfile", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileResponse)));
        }

        /// <summary>
        /// Update a Trace Filter object. Update a Trace Filter object. Any attribute missing from the request will be left unchanged.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x||||| traceFilterName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>MsgVpnTelemetryProfileTraceFilterResponse</returns>
        public MsgVpnTelemetryProfileTraceFilterResponse UpdateMsgVpnTelemetryProfileTraceFilter (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse> localVarResponse = UpdateMsgVpnTelemetryProfileTraceFilterWithHttpInfo(body, msgVpnName, telemetryProfileName, traceFilterName, opaquePassword, select);
             return localVarResponse.Data;
        }

        /// <summary>
        /// Update a Trace Filter object. Update a Trace Filter object. Any attribute missing from the request will be left unchanged.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x||||| traceFilterName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>ApiResponse of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        public ApiResponse< MsgVpnTelemetryProfileTraceFilterResponse > UpdateMsgVpnTelemetryProfileTraceFilterWithHttpInfo (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfileTraceFilter");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
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
                Exception exception = ExceptionFactory("UpdateMsgVpnTelemetryProfileTraceFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterResponse)));
        }

        /// <summary>
        /// Update a Trace Filter object. Update a Trace Filter object. Any attribute missing from the request will be left unchanged.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x||||| traceFilterName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of MsgVpnTelemetryProfileTraceFilterResponse</returns>
        public async System.Threading.Tasks.Task<MsgVpnTelemetryProfileTraceFilterResponse> UpdateMsgVpnTelemetryProfileTraceFilterAsync (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
             ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse> localVarResponse = await UpdateMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo(body, msgVpnName, telemetryProfileName, traceFilterName, opaquePassword, select);
             return localVarResponse.Data;

        }

        /// <summary>
        /// Update a Trace Filter object. Update a Trace Filter object. Any attribute missing from the request will be left unchanged.  A Trace Filter controls which messages received by the broker will be traced. If an incoming message matches an enabled tracing filter&#x27;s subscription, the message will be traced as it passes through the broker.   Attribute|Identifying|Read-Only|Write-Only|Requires-Disable|Auto-Disable|Deprecated|Opaque :- --|:- --|:- --|:- --|:- --|:- --|:- --|:- -- msgVpnName|x|x||||| telemetryProfileName|x|x||||| traceFilterName|x|x|||||    A SEMP client authorized with a minimum access scope/level of \&quot;vpn/read-write\&quot; is required to perform this operation.  This has been available since 2.31.
        /// </summary>
        /// <exception cref="Semp.V2.CSharp.Client.ApiException">Thrown when fails to make API call</exception>
        /// <param name="body">The Trace Filter object&#x27;s attributes.</param>
        /// <param name="msgVpnName">The name of the Message VPN.</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile.</param>
        /// <param name="traceFilterName">A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;.</param>
        /// <param name="opaquePassword">Accept opaque attributes in the request or return opaque attributes in the response, encrypted with the specified password. See the documentation for the &#x60;opaquePassword&#x60; parameter. (optional)</param>
        /// <param name="select">Include in the response only selected attributes of the object, or exclude from the response selected attributes of the object. See the documentation for the &#x60;select&#x60; parameter. (optional)</param>
        /// <returns>Task of ApiResponse (MsgVpnTelemetryProfileTraceFilterResponse)</returns>
        public async System.Threading.Tasks.Task<ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>> UpdateMsgVpnTelemetryProfileTraceFilterAsyncWithHttpInfo (MsgVpnTelemetryProfileTraceFilter body, string msgVpnName, string telemetryProfileName, string traceFilterName, string opaquePassword = null, List<string> select = null)
        {
            // verify the required parameter 'body' is set
            if (body == null)
                throw new ApiException(400, "Missing required parameter 'body' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'msgVpnName' is set
            if (msgVpnName == null)
                throw new ApiException(400, "Missing required parameter 'msgVpnName' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'telemetryProfileName' is set
            if (telemetryProfileName == null)
                throw new ApiException(400, "Missing required parameter 'telemetryProfileName' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfileTraceFilter");
            // verify the required parameter 'traceFilterName' is set
            if (traceFilterName == null)
                throw new ApiException(400, "Missing required parameter 'traceFilterName' when calling TelemetryProfileApi->UpdateMsgVpnTelemetryProfileTraceFilter");

            var localVarPath = "./msgVpns/{msgVpnName}/telemetryProfiles/{telemetryProfileName}/traceFilters/{traceFilterName}";
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
            if (telemetryProfileName != null) localVarPathParams.Add("telemetryProfileName", this.Configuration.ApiClient.ParameterToString(telemetryProfileName)); // path parameter
            if (traceFilterName != null) localVarPathParams.Add("traceFilterName", this.Configuration.ApiClient.ParameterToString(traceFilterName)); // path parameter
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
                Exception exception = ExceptionFactory("UpdateMsgVpnTelemetryProfileTraceFilter", localVarResponse);
                if (exception != null) throw exception;
            }

            return new ApiResponse<MsgVpnTelemetryProfileTraceFilterResponse>(localVarStatusCode,
                localVarResponse.Headers.ToDictionary(x => x.Key, x => string.Join(",", x.Value)),
                (MsgVpnTelemetryProfileTraceFilterResponse) this.Configuration.ApiClient.Deserialize(localVarResponse, typeof(MsgVpnTelemetryProfileTraceFilterResponse)));
        }

    }
}
