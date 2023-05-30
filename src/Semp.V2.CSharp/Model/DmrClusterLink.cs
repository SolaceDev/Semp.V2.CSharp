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
using System.Linq;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SwaggerDateConverter = Semp.V2.CSharp.Client.SwaggerDateConverter;

namespace Semp.V2.CSharp.Model
{
    /// <summary>
    /// DmrClusterLink
    /// </summary>
    [DataContract]
        public partial class DmrClusterLink :  IEquatable<DmrClusterLink>
    {
        /// <summary>
        /// The authentication scheme to be used by the Link which initiates connections to the remote node. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; 
        /// </summary>
        /// <value>The authentication scheme to be used by the Link which initiates connections to the remote node. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum AuthenticationSchemeEnum
        {
            /// <summary>
            /// Enum Basic for value: basic
            /// </summary>
            [EnumMember(Value = "basic")]
            Basic = 1,
            /// <summary>
            /// Enum ClientCertificate for value: client-certificate
            /// </summary>
            [EnumMember(Value = "client-certificate")]
            ClientCertificate = 2        }
        /// <summary>
        /// The authentication scheme to be used by the Link which initiates connections to the remote node. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; 
        /// </summary>
        /// <value>The authentication scheme to be used by the Link which initiates connections to the remote node. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; </value>
        [DataMember(Name="authenticationScheme", EmitDefaultValue=false)]
        public AuthenticationSchemeEnum? AuthenticationScheme { get; set; }
        /// <summary>
        /// The initiator of the Link&#x27;s TCP connections. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;lexical\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;lexical\&quot; - The \&quot;higher\&quot; node-name initiates. \&quot;local\&quot; - The local node initiates. \&quot;remote\&quot; - The remote node initiates. &lt;/pre&gt; 
        /// </summary>
        /// <value>The initiator of the Link&#x27;s TCP connections. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;lexical\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;lexical\&quot; - The \&quot;higher\&quot; node-name initiates. \&quot;local\&quot; - The local node initiates. \&quot;remote\&quot; - The remote node initiates. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum InitiatorEnum
        {
            /// <summary>
            /// Enum Lexical for value: lexical
            /// </summary>
            [EnumMember(Value = "lexical")]
            Lexical = 1,
            /// <summary>
            /// Enum Local for value: local
            /// </summary>
            [EnumMember(Value = "local")]
            Local = 2,
            /// <summary>
            /// Enum Remote for value: remote
            /// </summary>
            [EnumMember(Value = "remote")]
            Remote = 3        }
        /// <summary>
        /// The initiator of the Link&#x27;s TCP connections. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;lexical\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;lexical\&quot; - The \&quot;higher\&quot; node-name initiates. \&quot;local\&quot; - The local node initiates. \&quot;remote\&quot; - The remote node initiates. &lt;/pre&gt; 
        /// </summary>
        /// <value>The initiator of the Link&#x27;s TCP connections. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;lexical\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;lexical\&quot; - The \&quot;higher\&quot; node-name initiates. \&quot;local\&quot; - The local node initiates. \&quot;remote\&quot; - The remote node initiates. &lt;/pre&gt; </value>
        [DataMember(Name="initiator", EmitDefaultValue=false)]
        public InitiatorEnum? Initiator { get; set; }
        /// <summary>
        /// Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs cause the message to not be delivered to any destination and Transacted Session commits to fail. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;always\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt; 
        /// </summary>
        /// <value>Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs cause the message to not be delivered to any destination and Transacted Session commits to fail. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;always\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum QueueRejectMsgToSenderOnDiscardBehaviorEnum
        {
            /// <summary>
            /// Enum Always for value: always
            /// </summary>
            [EnumMember(Value = "always")]
            Always = 1,
            /// <summary>
            /// Enum WhenQueueEnabled for value: when-queue-enabled
            /// </summary>
            [EnumMember(Value = "when-queue-enabled")]
            WhenQueueEnabled = 2,
            /// <summary>
            /// Enum Never for value: never
            /// </summary>
            [EnumMember(Value = "never")]
            Never = 3        }
        /// <summary>
        /// Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs cause the message to not be delivered to any destination and Transacted Session commits to fail. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;always\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt; 
        /// </summary>
        /// <value>Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs cause the message to not be delivered to any destination and Transacted Session commits to fail. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;always\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt; </value>
        [DataMember(Name="queueRejectMsgToSenderOnDiscardBehavior", EmitDefaultValue=false)]
        public QueueRejectMsgToSenderOnDiscardBehaviorEnum? QueueRejectMsgToSenderOnDiscardBehavior { get; set; }
        /// <summary>
        /// The span of the Link, either internal or external. Internal Links connect nodes within the same Cluster. External Links connect nodes within different Clusters. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;external\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;internal\&quot; - Link to same cluster. \&quot;external\&quot; - Link to other cluster. &lt;/pre&gt; 
        /// </summary>
        /// <value>The span of the Link, either internal or external. Internal Links connect nodes within the same Cluster. External Links connect nodes within different Clusters. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;external\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;internal\&quot; - Link to same cluster. \&quot;external\&quot; - Link to other cluster. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum SpanEnum
        {
            /// <summary>
            /// Enum Internal for value: internal
            /// </summary>
            [EnumMember(Value = "internal")]
            Internal = 1,
            /// <summary>
            /// Enum External for value: external
            /// </summary>
            [EnumMember(Value = "external")]
            External = 2        }
        /// <summary>
        /// The span of the Link, either internal or external. Internal Links connect nodes within the same Cluster. External Links connect nodes within different Clusters. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;external\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;internal\&quot; - Link to same cluster. \&quot;external\&quot; - Link to other cluster. &lt;/pre&gt; 
        /// </summary>
        /// <value>The span of the Link, either internal or external. Internal Links connect nodes within the same Cluster. External Links connect nodes within different Clusters. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;external\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;internal\&quot; - Link to same cluster. \&quot;external\&quot; - Link to other cluster. &lt;/pre&gt; </value>
        [DataMember(Name="span", EmitDefaultValue=false)]
        public SpanEnum? Span { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="DmrClusterLink" /> class.
        /// </summary>
        /// <param name="authenticationBasicPassword">The password used to authenticate with the remote node when using basic internal authentication. If this per-Link password is not configured, the Cluster&#x27;s password is used instead. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="authenticationScheme">The authentication scheme to be used by the Link which initiates connections to the remote node. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;basic\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;basic\&quot; - Basic Authentication Scheme (via username and password). \&quot;client-certificate\&quot; - Client Certificate Authentication Scheme (via certificate file or content). &lt;/pre&gt; .</param>
        /// <param name="clientProfileQueueControl1MaxDepth">The maximum depth of the \&quot;Control 1\&quot; (C-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;..</param>
        /// <param name="clientProfileQueueControl1MinMsgBurst">The number of messages that are always allowed entry into the \&quot;Control 1\&quot; (C-1) priority queue, regardless of the &#x60;clientProfileQueueControl1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;4&#x60;..</param>
        /// <param name="clientProfileQueueDirect1MaxDepth">The maximum depth of the \&quot;Direct 1\&quot; (D-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;..</param>
        /// <param name="clientProfileQueueDirect1MinMsgBurst">The number of messages that are always allowed entry into the \&quot;Direct 1\&quot; (D-1) priority queue, regardless of the &#x60;clientProfileQueueDirect1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;4&#x60;..</param>
        /// <param name="clientProfileQueueDirect2MaxDepth">The maximum depth of the \&quot;Direct 2\&quot; (D-2) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;..</param>
        /// <param name="clientProfileQueueDirect2MinMsgBurst">The number of messages that are always allowed entry into the \&quot;Direct 2\&quot; (D-2) priority queue, regardless of the &#x60;clientProfileQueueDirect2MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;4&#x60;..</param>
        /// <param name="clientProfileQueueDirect3MaxDepth">The maximum depth of the \&quot;Direct 3\&quot; (D-3) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;..</param>
        /// <param name="clientProfileQueueDirect3MinMsgBurst">The number of messages that are always allowed entry into the \&quot;Direct 3\&quot; (D-3) priority queue, regardless of the &#x60;clientProfileQueueDirect3MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;4&#x60;..</param>
        /// <param name="clientProfileQueueGuaranteed1MaxDepth">The maximum depth of the \&quot;Guaranteed 1\&quot; (G-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;..</param>
        /// <param name="clientProfileQueueGuaranteed1MinMsgBurst">The number of messages that are always allowed entry into the \&quot;Guaranteed 1\&quot; (G-3) priority queue, regardless of the &#x60;clientProfileQueueGuaranteed1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;255&#x60;..</param>
        /// <param name="clientProfileTcpCongestionWindowSize">The TCP initial congestion window size, in multiples of the TCP Maximum Segment Size (MSS). Changing the value from its default of 2 results in non-compliance with RFC 2581. Contact support before changing this value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;2&#x60;..</param>
        /// <param name="clientProfileTcpKeepaliveCount">The number of TCP keepalive retransmissions to be carried out before declaring that the remote end is not available. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;5&#x60;..</param>
        /// <param name="clientProfileTcpKeepaliveIdleTime">The amount of time a connection must remain idle before TCP begins sending keepalive probes, in seconds. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;3&#x60;..</param>
        /// <param name="clientProfileTcpKeepaliveInterval">The amount of time between TCP keepalive retransmissions when no acknowledgement is received, in seconds. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;..</param>
        /// <param name="clientProfileTcpMaxSegmentSize">The TCP maximum segment size, in bytes. Changes are applied to all existing connections. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1460&#x60;..</param>
        /// <param name="clientProfileTcpMaxWindowSize">The TCP maximum window size, in kilobytes. Changes are applied to all existing connections. This setting is ignored on the software broker. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;256&#x60;..</param>
        /// <param name="dmrClusterName">The name of the Cluster..</param>
        /// <param name="egressFlowWindowSize">The number of outstanding guaranteed messages that can be sent over the Link before acknowledgement is received by the sender. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;255&#x60;..</param>
        /// <param name="enabled">Enable or disable the Link. When disabled, subscription sets of this and the remote node are not kept up-to-date, and messages are not exchanged with the remote node. Published guaranteed messages will be queued up for future delivery based on current subscription sets. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="initiator">The initiator of the Link&#x27;s TCP connections. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;lexical\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;lexical\&quot; - The \&quot;higher\&quot; node-name initiates. \&quot;local\&quot; - The local node initiates. \&quot;remote\&quot; - The remote node initiates. &lt;/pre&gt; .</param>
        /// <param name="queueDeadMsgQueue">The name of the Dead Message Queue (DMQ) used by the Queue for discarded messages. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;#DEAD_MSG_QUEUE\&quot;&#x60;..</param>
        /// <param name="queueEventSpoolUsageThreshold">queueEventSpoolUsageThreshold.</param>
        /// <param name="queueMaxDeliveredUnackedMsgsPerFlow">The maximum number of messages delivered but not acknowledged per flow for the Queue. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1000000&#x60;..</param>
        /// <param name="queueMaxMsgSpoolUsage">The maximum message spool usage by the Queue (quota), in megabytes (MB). Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;800000&#x60;..</param>
        /// <param name="queueMaxRedeliveryCount">The maximum number of times the Queue will attempt redelivery of a message prior to it being discarded or moved to the DMQ. A value of 0 means to retry forever. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;..</param>
        /// <param name="queueMaxTtl">The maximum time in seconds a message can stay in the Queue when &#x60;queueRespectTtlEnabled&#x60; is &#x60;true&#x60;. A message expires when the lesser of the sender assigned time-to-live (TTL) in the message and the &#x60;queueMaxTtl&#x60; configured for the Queue, is exceeded. A value of 0 disables expiry. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;..</param>
        /// <param name="queueRejectMsgToSenderOnDiscardBehavior">Determines when to return negative acknowledgements (NACKs) to sending clients on message discards. Note that NACKs cause the message to not be delivered to any destination and Transacted Session commits to fail. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;always\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;always\&quot; - Always return a negative acknowledgment (NACK) to the sending client on message discard. \&quot;when-queue-enabled\&quot; - Only return a negative acknowledgment (NACK) to the sending client on message discard when the Queue is enabled. \&quot;never\&quot; - Never return a negative acknowledgment (NACK) to the sending client on message discard. &lt;/pre&gt; .</param>
        /// <param name="queueRespectTtlEnabled">Enable or disable the respecting of the time-to-live (TTL) for messages in the Queue. When enabled, expired messages are discarded or moved to the DMQ. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="remoteNodeName">The name of the node at the remote end of the Link..</param>
        /// <param name="span">The span of the Link, either internal or external. Internal Links connect nodes within the same Cluster. External Links connect nodes within different Clusters. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;external\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;internal\&quot; - Link to same cluster. \&quot;external\&quot; - Link to other cluster. &lt;/pre&gt; .</param>
        /// <param name="transportCompressedEnabled">Enable or disable compression on the Link. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="transportTlsEnabled">Enable or disable encryption (TLS) on the Link. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;..</param>
        public DmrClusterLink(string authenticationBasicPassword = default(string), AuthenticationSchemeEnum? authenticationScheme = default(AuthenticationSchemeEnum?), int? clientProfileQueueControl1MaxDepth = default(int?), int? clientProfileQueueControl1MinMsgBurst = default(int?), int? clientProfileQueueDirect1MaxDepth = default(int?), int? clientProfileQueueDirect1MinMsgBurst = default(int?), int? clientProfileQueueDirect2MaxDepth = default(int?), int? clientProfileQueueDirect2MinMsgBurst = default(int?), int? clientProfileQueueDirect3MaxDepth = default(int?), int? clientProfileQueueDirect3MinMsgBurst = default(int?), int? clientProfileQueueGuaranteed1MaxDepth = default(int?), int? clientProfileQueueGuaranteed1MinMsgBurst = default(int?), long? clientProfileTcpCongestionWindowSize = default(long?), long? clientProfileTcpKeepaliveCount = default(long?), long? clientProfileTcpKeepaliveIdleTime = default(long?), long? clientProfileTcpKeepaliveInterval = default(long?), long? clientProfileTcpMaxSegmentSize = default(long?), long? clientProfileTcpMaxWindowSize = default(long?), string dmrClusterName = default(string), long? egressFlowWindowSize = default(long?), bool? enabled = default(bool?), InitiatorEnum? initiator = default(InitiatorEnum?), string queueDeadMsgQueue = default(string), EventThreshold queueEventSpoolUsageThreshold = default(EventThreshold), long? queueMaxDeliveredUnackedMsgsPerFlow = default(long?), long? queueMaxMsgSpoolUsage = default(long?), long? queueMaxRedeliveryCount = default(long?), long? queueMaxTtl = default(long?), QueueRejectMsgToSenderOnDiscardBehaviorEnum? queueRejectMsgToSenderOnDiscardBehavior = default(QueueRejectMsgToSenderOnDiscardBehaviorEnum?), bool? queueRespectTtlEnabled = default(bool?), string remoteNodeName = default(string), SpanEnum? span = default(SpanEnum?), bool? transportCompressedEnabled = default(bool?), bool? transportTlsEnabled = default(bool?))
        {
            this.AuthenticationBasicPassword = authenticationBasicPassword;
            this.AuthenticationScheme = authenticationScheme;
            this.ClientProfileQueueControl1MaxDepth = clientProfileQueueControl1MaxDepth;
            this.ClientProfileQueueControl1MinMsgBurst = clientProfileQueueControl1MinMsgBurst;
            this.ClientProfileQueueDirect1MaxDepth = clientProfileQueueDirect1MaxDepth;
            this.ClientProfileQueueDirect1MinMsgBurst = clientProfileQueueDirect1MinMsgBurst;
            this.ClientProfileQueueDirect2MaxDepth = clientProfileQueueDirect2MaxDepth;
            this.ClientProfileQueueDirect2MinMsgBurst = clientProfileQueueDirect2MinMsgBurst;
            this.ClientProfileQueueDirect3MaxDepth = clientProfileQueueDirect3MaxDepth;
            this.ClientProfileQueueDirect3MinMsgBurst = clientProfileQueueDirect3MinMsgBurst;
            this.ClientProfileQueueGuaranteed1MaxDepth = clientProfileQueueGuaranteed1MaxDepth;
            this.ClientProfileQueueGuaranteed1MinMsgBurst = clientProfileQueueGuaranteed1MinMsgBurst;
            this.ClientProfileTcpCongestionWindowSize = clientProfileTcpCongestionWindowSize;
            this.ClientProfileTcpKeepaliveCount = clientProfileTcpKeepaliveCount;
            this.ClientProfileTcpKeepaliveIdleTime = clientProfileTcpKeepaliveIdleTime;
            this.ClientProfileTcpKeepaliveInterval = clientProfileTcpKeepaliveInterval;
            this.ClientProfileTcpMaxSegmentSize = clientProfileTcpMaxSegmentSize;
            this.ClientProfileTcpMaxWindowSize = clientProfileTcpMaxWindowSize;
            this.DmrClusterName = dmrClusterName;
            this.EgressFlowWindowSize = egressFlowWindowSize;
            this.Enabled = enabled;
            this.Initiator = initiator;
            this.QueueDeadMsgQueue = queueDeadMsgQueue;
            this.QueueEventSpoolUsageThreshold = queueEventSpoolUsageThreshold;
            this.QueueMaxDeliveredUnackedMsgsPerFlow = queueMaxDeliveredUnackedMsgsPerFlow;
            this.QueueMaxMsgSpoolUsage = queueMaxMsgSpoolUsage;
            this.QueueMaxRedeliveryCount = queueMaxRedeliveryCount;
            this.QueueMaxTtl = queueMaxTtl;
            this.QueueRejectMsgToSenderOnDiscardBehavior = queueRejectMsgToSenderOnDiscardBehavior;
            this.QueueRespectTtlEnabled = queueRespectTtlEnabled;
            this.RemoteNodeName = remoteNodeName;
            this.Span = span;
            this.TransportCompressedEnabled = transportCompressedEnabled;
            this.TransportTlsEnabled = transportTlsEnabled;
        }
        
        /// <summary>
        /// The password used to authenticate with the remote node when using basic internal authentication. If this per-Link password is not configured, the Cluster&#x27;s password is used instead. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The password used to authenticate with the remote node when using basic internal authentication. If this per-Link password is not configured, the Cluster&#x27;s password is used instead. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="authenticationBasicPassword", EmitDefaultValue=false)]
        public string AuthenticationBasicPassword { get; set; }


        /// <summary>
        /// The maximum depth of the \&quot;Control 1\&quot; (C-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;.
        /// </summary>
        /// <value>The maximum depth of the \&quot;Control 1\&quot; (C-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;.</value>
        [DataMember(Name="clientProfileQueueControl1MaxDepth", EmitDefaultValue=false)]
        public int? ClientProfileQueueControl1MaxDepth { get; set; }

        /// <summary>
        /// The number of messages that are always allowed entry into the \&quot;Control 1\&quot; (C-1) priority queue, regardless of the &#x60;clientProfileQueueControl1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;4&#x60;.
        /// </summary>
        /// <value>The number of messages that are always allowed entry into the \&quot;Control 1\&quot; (C-1) priority queue, regardless of the &#x60;clientProfileQueueControl1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;4&#x60;.</value>
        [DataMember(Name="clientProfileQueueControl1MinMsgBurst", EmitDefaultValue=false)]
        public int? ClientProfileQueueControl1MinMsgBurst { get; set; }

        /// <summary>
        /// The maximum depth of the \&quot;Direct 1\&quot; (D-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;.
        /// </summary>
        /// <value>The maximum depth of the \&quot;Direct 1\&quot; (D-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;.</value>
        [DataMember(Name="clientProfileQueueDirect1MaxDepth", EmitDefaultValue=false)]
        public int? ClientProfileQueueDirect1MaxDepth { get; set; }

        /// <summary>
        /// The number of messages that are always allowed entry into the \&quot;Direct 1\&quot; (D-1) priority queue, regardless of the &#x60;clientProfileQueueDirect1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;4&#x60;.
        /// </summary>
        /// <value>The number of messages that are always allowed entry into the \&quot;Direct 1\&quot; (D-1) priority queue, regardless of the &#x60;clientProfileQueueDirect1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;4&#x60;.</value>
        [DataMember(Name="clientProfileQueueDirect1MinMsgBurst", EmitDefaultValue=false)]
        public int? ClientProfileQueueDirect1MinMsgBurst { get; set; }

        /// <summary>
        /// The maximum depth of the \&quot;Direct 2\&quot; (D-2) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;.
        /// </summary>
        /// <value>The maximum depth of the \&quot;Direct 2\&quot; (D-2) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;.</value>
        [DataMember(Name="clientProfileQueueDirect2MaxDepth", EmitDefaultValue=false)]
        public int? ClientProfileQueueDirect2MaxDepth { get; set; }

        /// <summary>
        /// The number of messages that are always allowed entry into the \&quot;Direct 2\&quot; (D-2) priority queue, regardless of the &#x60;clientProfileQueueDirect2MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;4&#x60;.
        /// </summary>
        /// <value>The number of messages that are always allowed entry into the \&quot;Direct 2\&quot; (D-2) priority queue, regardless of the &#x60;clientProfileQueueDirect2MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;4&#x60;.</value>
        [DataMember(Name="clientProfileQueueDirect2MinMsgBurst", EmitDefaultValue=false)]
        public int? ClientProfileQueueDirect2MinMsgBurst { get; set; }

        /// <summary>
        /// The maximum depth of the \&quot;Direct 3\&quot; (D-3) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;.
        /// </summary>
        /// <value>The maximum depth of the \&quot;Direct 3\&quot; (D-3) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;.</value>
        [DataMember(Name="clientProfileQueueDirect3MaxDepth", EmitDefaultValue=false)]
        public int? ClientProfileQueueDirect3MaxDepth { get; set; }

        /// <summary>
        /// The number of messages that are always allowed entry into the \&quot;Direct 3\&quot; (D-3) priority queue, regardless of the &#x60;clientProfileQueueDirect3MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;4&#x60;.
        /// </summary>
        /// <value>The number of messages that are always allowed entry into the \&quot;Direct 3\&quot; (D-3) priority queue, regardless of the &#x60;clientProfileQueueDirect3MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;4&#x60;.</value>
        [DataMember(Name="clientProfileQueueDirect3MinMsgBurst", EmitDefaultValue=false)]
        public int? ClientProfileQueueDirect3MinMsgBurst { get; set; }

        /// <summary>
        /// The maximum depth of the \&quot;Guaranteed 1\&quot; (G-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;.
        /// </summary>
        /// <value>The maximum depth of the \&quot;Guaranteed 1\&quot; (G-1) priority queue, in work units. Each work unit is 2048 bytes of message data. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;20000&#x60;.</value>
        [DataMember(Name="clientProfileQueueGuaranteed1MaxDepth", EmitDefaultValue=false)]
        public int? ClientProfileQueueGuaranteed1MaxDepth { get; set; }

        /// <summary>
        /// The number of messages that are always allowed entry into the \&quot;Guaranteed 1\&quot; (G-3) priority queue, regardless of the &#x60;clientProfileQueueGuaranteed1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;255&#x60;.
        /// </summary>
        /// <value>The number of messages that are always allowed entry into the \&quot;Guaranteed 1\&quot; (G-3) priority queue, regardless of the &#x60;clientProfileQueueGuaranteed1MaxDepth&#x60; value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;255&#x60;.</value>
        [DataMember(Name="clientProfileQueueGuaranteed1MinMsgBurst", EmitDefaultValue=false)]
        public int? ClientProfileQueueGuaranteed1MinMsgBurst { get; set; }

        /// <summary>
        /// The TCP initial congestion window size, in multiples of the TCP Maximum Segment Size (MSS). Changing the value from its default of 2 results in non-compliance with RFC 2581. Contact support before changing this value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;2&#x60;.
        /// </summary>
        /// <value>The TCP initial congestion window size, in multiples of the TCP Maximum Segment Size (MSS). Changing the value from its default of 2 results in non-compliance with RFC 2581. Contact support before changing this value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;2&#x60;.</value>
        [DataMember(Name="clientProfileTcpCongestionWindowSize", EmitDefaultValue=false)]
        public long? ClientProfileTcpCongestionWindowSize { get; set; }

        /// <summary>
        /// The number of TCP keepalive retransmissions to be carried out before declaring that the remote end is not available. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;5&#x60;.
        /// </summary>
        /// <value>The number of TCP keepalive retransmissions to be carried out before declaring that the remote end is not available. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;5&#x60;.</value>
        [DataMember(Name="clientProfileTcpKeepaliveCount", EmitDefaultValue=false)]
        public long? ClientProfileTcpKeepaliveCount { get; set; }

        /// <summary>
        /// The amount of time a connection must remain idle before TCP begins sending keepalive probes, in seconds. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;3&#x60;.
        /// </summary>
        /// <value>The amount of time a connection must remain idle before TCP begins sending keepalive probes, in seconds. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;3&#x60;.</value>
        [DataMember(Name="clientProfileTcpKeepaliveIdleTime", EmitDefaultValue=false)]
        public long? ClientProfileTcpKeepaliveIdleTime { get; set; }

        /// <summary>
        /// The amount of time between TCP keepalive retransmissions when no acknowledgement is received, in seconds. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;.
        /// </summary>
        /// <value>The amount of time between TCP keepalive retransmissions when no acknowledgement is received, in seconds. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1&#x60;.</value>
        [DataMember(Name="clientProfileTcpKeepaliveInterval", EmitDefaultValue=false)]
        public long? ClientProfileTcpKeepaliveInterval { get; set; }

        /// <summary>
        /// The TCP maximum segment size, in bytes. Changes are applied to all existing connections. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1460&#x60;.
        /// </summary>
        /// <value>The TCP maximum segment size, in bytes. Changes are applied to all existing connections. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1460&#x60;.</value>
        [DataMember(Name="clientProfileTcpMaxSegmentSize", EmitDefaultValue=false)]
        public long? ClientProfileTcpMaxSegmentSize { get; set; }

        /// <summary>
        /// The TCP maximum window size, in kilobytes. Changes are applied to all existing connections. This setting is ignored on the software broker. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;256&#x60;.
        /// </summary>
        /// <value>The TCP maximum window size, in kilobytes. Changes are applied to all existing connections. This setting is ignored on the software broker. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;256&#x60;.</value>
        [DataMember(Name="clientProfileTcpMaxWindowSize", EmitDefaultValue=false)]
        public long? ClientProfileTcpMaxWindowSize { get; set; }

        /// <summary>
        /// The name of the Cluster.
        /// </summary>
        /// <value>The name of the Cluster.</value>
        [DataMember(Name="dmrClusterName", EmitDefaultValue=false)]
        public string DmrClusterName { get; set; }

        /// <summary>
        /// The number of outstanding guaranteed messages that can be sent over the Link before acknowledgement is received by the sender. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;255&#x60;.
        /// </summary>
        /// <value>The number of outstanding guaranteed messages that can be sent over the Link before acknowledgement is received by the sender. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;255&#x60;.</value>
        [DataMember(Name="egressFlowWindowSize", EmitDefaultValue=false)]
        public long? EgressFlowWindowSize { get; set; }

        /// <summary>
        /// Enable or disable the Link. When disabled, subscription sets of this and the remote node are not kept up-to-date, and messages are not exchanged with the remote node. Published guaranteed messages will be queued up for future delivery based on current subscription sets. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the Link. When disabled, subscription sets of this and the remote node are not kept up-to-date, and messages are not exchanged with the remote node. Published guaranteed messages will be queued up for future delivery based on current subscription sets. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="enabled", EmitDefaultValue=false)]
        public bool? Enabled { get; set; }


        /// <summary>
        /// The name of the Dead Message Queue (DMQ) used by the Queue for discarded messages. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;#DEAD_MSG_QUEUE\&quot;&#x60;.
        /// </summary>
        /// <value>The name of the Dead Message Queue (DMQ) used by the Queue for discarded messages. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;#DEAD_MSG_QUEUE\&quot;&#x60;.</value>
        [DataMember(Name="queueDeadMsgQueue", EmitDefaultValue=false)]
        public string QueueDeadMsgQueue { get; set; }

        /// <summary>
        /// Gets or Sets QueueEventSpoolUsageThreshold
        /// </summary>
        [DataMember(Name="queueEventSpoolUsageThreshold", EmitDefaultValue=false)]
        public EventThreshold QueueEventSpoolUsageThreshold { get; set; }

        /// <summary>
        /// The maximum number of messages delivered but not acknowledged per flow for the Queue. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1000000&#x60;.
        /// </summary>
        /// <value>The maximum number of messages delivered but not acknowledged per flow for the Queue. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1000000&#x60;.</value>
        [DataMember(Name="queueMaxDeliveredUnackedMsgsPerFlow", EmitDefaultValue=false)]
        public long? QueueMaxDeliveredUnackedMsgsPerFlow { get; set; }

        /// <summary>
        /// The maximum message spool usage by the Queue (quota), in megabytes (MB). Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;800000&#x60;.
        /// </summary>
        /// <value>The maximum message spool usage by the Queue (quota), in megabytes (MB). Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;800000&#x60;.</value>
        [DataMember(Name="queueMaxMsgSpoolUsage", EmitDefaultValue=false)]
        public long? QueueMaxMsgSpoolUsage { get; set; }

        /// <summary>
        /// The maximum number of times the Queue will attempt redelivery of a message prior to it being discarded or moved to the DMQ. A value of 0 means to retry forever. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;.
        /// </summary>
        /// <value>The maximum number of times the Queue will attempt redelivery of a message prior to it being discarded or moved to the DMQ. A value of 0 means to retry forever. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;.</value>
        [DataMember(Name="queueMaxRedeliveryCount", EmitDefaultValue=false)]
        public long? QueueMaxRedeliveryCount { get; set; }

        /// <summary>
        /// The maximum time in seconds a message can stay in the Queue when &#x60;queueRespectTtlEnabled&#x60; is &#x60;true&#x60;. A message expires when the lesser of the sender assigned time-to-live (TTL) in the message and the &#x60;queueMaxTtl&#x60; configured for the Queue, is exceeded. A value of 0 disables expiry. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;.
        /// </summary>
        /// <value>The maximum time in seconds a message can stay in the Queue when &#x60;queueRespectTtlEnabled&#x60; is &#x60;true&#x60;. A message expires when the lesser of the sender assigned time-to-live (TTL) in the message and the &#x60;queueMaxTtl&#x60; configured for the Queue, is exceeded. A value of 0 disables expiry. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;.</value>
        [DataMember(Name="queueMaxTtl", EmitDefaultValue=false)]
        public long? QueueMaxTtl { get; set; }


        /// <summary>
        /// Enable or disable the respecting of the time-to-live (TTL) for messages in the Queue. When enabled, expired messages are discarded or moved to the DMQ. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the respecting of the time-to-live (TTL) for messages in the Queue. When enabled, expired messages are discarded or moved to the DMQ. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="queueRespectTtlEnabled", EmitDefaultValue=false)]
        public bool? QueueRespectTtlEnabled { get; set; }

        /// <summary>
        /// The name of the node at the remote end of the Link.
        /// </summary>
        /// <value>The name of the node at the remote end of the Link.</value>
        [DataMember(Name="remoteNodeName", EmitDefaultValue=false)]
        public string RemoteNodeName { get; set; }


        /// <summary>
        /// Enable or disable compression on the Link. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable compression on the Link. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="transportCompressedEnabled", EmitDefaultValue=false)]
        public bool? TransportCompressedEnabled { get; set; }

        /// <summary>
        /// Enable or disable encryption (TLS) on the Link. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable encryption (TLS) on the Link. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as enabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="transportTlsEnabled", EmitDefaultValue=false)]
        public bool? TransportTlsEnabled { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class DmrClusterLink {\n");
            sb.Append("  AuthenticationBasicPassword: ").Append(AuthenticationBasicPassword).Append("\n");
            sb.Append("  AuthenticationScheme: ").Append(AuthenticationScheme).Append("\n");
            sb.Append("  ClientProfileQueueControl1MaxDepth: ").Append(ClientProfileQueueControl1MaxDepth).Append("\n");
            sb.Append("  ClientProfileQueueControl1MinMsgBurst: ").Append(ClientProfileQueueControl1MinMsgBurst).Append("\n");
            sb.Append("  ClientProfileQueueDirect1MaxDepth: ").Append(ClientProfileQueueDirect1MaxDepth).Append("\n");
            sb.Append("  ClientProfileQueueDirect1MinMsgBurst: ").Append(ClientProfileQueueDirect1MinMsgBurst).Append("\n");
            sb.Append("  ClientProfileQueueDirect2MaxDepth: ").Append(ClientProfileQueueDirect2MaxDepth).Append("\n");
            sb.Append("  ClientProfileQueueDirect2MinMsgBurst: ").Append(ClientProfileQueueDirect2MinMsgBurst).Append("\n");
            sb.Append("  ClientProfileQueueDirect3MaxDepth: ").Append(ClientProfileQueueDirect3MaxDepth).Append("\n");
            sb.Append("  ClientProfileQueueDirect3MinMsgBurst: ").Append(ClientProfileQueueDirect3MinMsgBurst).Append("\n");
            sb.Append("  ClientProfileQueueGuaranteed1MaxDepth: ").Append(ClientProfileQueueGuaranteed1MaxDepth).Append("\n");
            sb.Append("  ClientProfileQueueGuaranteed1MinMsgBurst: ").Append(ClientProfileQueueGuaranteed1MinMsgBurst).Append("\n");
            sb.Append("  ClientProfileTcpCongestionWindowSize: ").Append(ClientProfileTcpCongestionWindowSize).Append("\n");
            sb.Append("  ClientProfileTcpKeepaliveCount: ").Append(ClientProfileTcpKeepaliveCount).Append("\n");
            sb.Append("  ClientProfileTcpKeepaliveIdleTime: ").Append(ClientProfileTcpKeepaliveIdleTime).Append("\n");
            sb.Append("  ClientProfileTcpKeepaliveInterval: ").Append(ClientProfileTcpKeepaliveInterval).Append("\n");
            sb.Append("  ClientProfileTcpMaxSegmentSize: ").Append(ClientProfileTcpMaxSegmentSize).Append("\n");
            sb.Append("  ClientProfileTcpMaxWindowSize: ").Append(ClientProfileTcpMaxWindowSize).Append("\n");
            sb.Append("  DmrClusterName: ").Append(DmrClusterName).Append("\n");
            sb.Append("  EgressFlowWindowSize: ").Append(EgressFlowWindowSize).Append("\n");
            sb.Append("  Enabled: ").Append(Enabled).Append("\n");
            sb.Append("  Initiator: ").Append(Initiator).Append("\n");
            sb.Append("  QueueDeadMsgQueue: ").Append(QueueDeadMsgQueue).Append("\n");
            sb.Append("  QueueEventSpoolUsageThreshold: ").Append(QueueEventSpoolUsageThreshold).Append("\n");
            sb.Append("  QueueMaxDeliveredUnackedMsgsPerFlow: ").Append(QueueMaxDeliveredUnackedMsgsPerFlow).Append("\n");
            sb.Append("  QueueMaxMsgSpoolUsage: ").Append(QueueMaxMsgSpoolUsage).Append("\n");
            sb.Append("  QueueMaxRedeliveryCount: ").Append(QueueMaxRedeliveryCount).Append("\n");
            sb.Append("  QueueMaxTtl: ").Append(QueueMaxTtl).Append("\n");
            sb.Append("  QueueRejectMsgToSenderOnDiscardBehavior: ").Append(QueueRejectMsgToSenderOnDiscardBehavior).Append("\n");
            sb.Append("  QueueRespectTtlEnabled: ").Append(QueueRespectTtlEnabled).Append("\n");
            sb.Append("  RemoteNodeName: ").Append(RemoteNodeName).Append("\n");
            sb.Append("  Span: ").Append(Span).Append("\n");
            sb.Append("  TransportCompressedEnabled: ").Append(TransportCompressedEnabled).Append("\n");
            sb.Append("  TransportTlsEnabled: ").Append(TransportTlsEnabled).Append("\n");
            sb.Append("}\n");
            return sb.ToString();
        }
  
        /// <summary>
        /// Returns the JSON string presentation of the object
        /// </summary>
        /// <returns>JSON string presentation of the object</returns>
        public virtual string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }

        /// <summary>
        /// Returns true if objects are equal
        /// </summary>
        /// <param name="input">Object to be compared</param>
        /// <returns>Boolean</returns>
        public override bool Equals(object input)
        {
            return this.Equals(input as DmrClusterLink);
        }

        /// <summary>
        /// Returns true if DmrClusterLink instances are equal
        /// </summary>
        /// <param name="input">Instance of DmrClusterLink to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(DmrClusterLink input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.AuthenticationBasicPassword == input.AuthenticationBasicPassword ||
                    (this.AuthenticationBasicPassword != null &&
                    this.AuthenticationBasicPassword.Equals(input.AuthenticationBasicPassword))
                ) && 
                (
                    this.AuthenticationScheme == input.AuthenticationScheme ||
                    (this.AuthenticationScheme != null &&
                    this.AuthenticationScheme.Equals(input.AuthenticationScheme))
                ) && 
                (
                    this.ClientProfileQueueControl1MaxDepth == input.ClientProfileQueueControl1MaxDepth ||
                    (this.ClientProfileQueueControl1MaxDepth != null &&
                    this.ClientProfileQueueControl1MaxDepth.Equals(input.ClientProfileQueueControl1MaxDepth))
                ) && 
                (
                    this.ClientProfileQueueControl1MinMsgBurst == input.ClientProfileQueueControl1MinMsgBurst ||
                    (this.ClientProfileQueueControl1MinMsgBurst != null &&
                    this.ClientProfileQueueControl1MinMsgBurst.Equals(input.ClientProfileQueueControl1MinMsgBurst))
                ) && 
                (
                    this.ClientProfileQueueDirect1MaxDepth == input.ClientProfileQueueDirect1MaxDepth ||
                    (this.ClientProfileQueueDirect1MaxDepth != null &&
                    this.ClientProfileQueueDirect1MaxDepth.Equals(input.ClientProfileQueueDirect1MaxDepth))
                ) && 
                (
                    this.ClientProfileQueueDirect1MinMsgBurst == input.ClientProfileQueueDirect1MinMsgBurst ||
                    (this.ClientProfileQueueDirect1MinMsgBurst != null &&
                    this.ClientProfileQueueDirect1MinMsgBurst.Equals(input.ClientProfileQueueDirect1MinMsgBurst))
                ) && 
                (
                    this.ClientProfileQueueDirect2MaxDepth == input.ClientProfileQueueDirect2MaxDepth ||
                    (this.ClientProfileQueueDirect2MaxDepth != null &&
                    this.ClientProfileQueueDirect2MaxDepth.Equals(input.ClientProfileQueueDirect2MaxDepth))
                ) && 
                (
                    this.ClientProfileQueueDirect2MinMsgBurst == input.ClientProfileQueueDirect2MinMsgBurst ||
                    (this.ClientProfileQueueDirect2MinMsgBurst != null &&
                    this.ClientProfileQueueDirect2MinMsgBurst.Equals(input.ClientProfileQueueDirect2MinMsgBurst))
                ) && 
                (
                    this.ClientProfileQueueDirect3MaxDepth == input.ClientProfileQueueDirect3MaxDepth ||
                    (this.ClientProfileQueueDirect3MaxDepth != null &&
                    this.ClientProfileQueueDirect3MaxDepth.Equals(input.ClientProfileQueueDirect3MaxDepth))
                ) && 
                (
                    this.ClientProfileQueueDirect3MinMsgBurst == input.ClientProfileQueueDirect3MinMsgBurst ||
                    (this.ClientProfileQueueDirect3MinMsgBurst != null &&
                    this.ClientProfileQueueDirect3MinMsgBurst.Equals(input.ClientProfileQueueDirect3MinMsgBurst))
                ) && 
                (
                    this.ClientProfileQueueGuaranteed1MaxDepth == input.ClientProfileQueueGuaranteed1MaxDepth ||
                    (this.ClientProfileQueueGuaranteed1MaxDepth != null &&
                    this.ClientProfileQueueGuaranteed1MaxDepth.Equals(input.ClientProfileQueueGuaranteed1MaxDepth))
                ) && 
                (
                    this.ClientProfileQueueGuaranteed1MinMsgBurst == input.ClientProfileQueueGuaranteed1MinMsgBurst ||
                    (this.ClientProfileQueueGuaranteed1MinMsgBurst != null &&
                    this.ClientProfileQueueGuaranteed1MinMsgBurst.Equals(input.ClientProfileQueueGuaranteed1MinMsgBurst))
                ) && 
                (
                    this.ClientProfileTcpCongestionWindowSize == input.ClientProfileTcpCongestionWindowSize ||
                    (this.ClientProfileTcpCongestionWindowSize != null &&
                    this.ClientProfileTcpCongestionWindowSize.Equals(input.ClientProfileTcpCongestionWindowSize))
                ) && 
                (
                    this.ClientProfileTcpKeepaliveCount == input.ClientProfileTcpKeepaliveCount ||
                    (this.ClientProfileTcpKeepaliveCount != null &&
                    this.ClientProfileTcpKeepaliveCount.Equals(input.ClientProfileTcpKeepaliveCount))
                ) && 
                (
                    this.ClientProfileTcpKeepaliveIdleTime == input.ClientProfileTcpKeepaliveIdleTime ||
                    (this.ClientProfileTcpKeepaliveIdleTime != null &&
                    this.ClientProfileTcpKeepaliveIdleTime.Equals(input.ClientProfileTcpKeepaliveIdleTime))
                ) && 
                (
                    this.ClientProfileTcpKeepaliveInterval == input.ClientProfileTcpKeepaliveInterval ||
                    (this.ClientProfileTcpKeepaliveInterval != null &&
                    this.ClientProfileTcpKeepaliveInterval.Equals(input.ClientProfileTcpKeepaliveInterval))
                ) && 
                (
                    this.ClientProfileTcpMaxSegmentSize == input.ClientProfileTcpMaxSegmentSize ||
                    (this.ClientProfileTcpMaxSegmentSize != null &&
                    this.ClientProfileTcpMaxSegmentSize.Equals(input.ClientProfileTcpMaxSegmentSize))
                ) && 
                (
                    this.ClientProfileTcpMaxWindowSize == input.ClientProfileTcpMaxWindowSize ||
                    (this.ClientProfileTcpMaxWindowSize != null &&
                    this.ClientProfileTcpMaxWindowSize.Equals(input.ClientProfileTcpMaxWindowSize))
                ) && 
                (
                    this.DmrClusterName == input.DmrClusterName ||
                    (this.DmrClusterName != null &&
                    this.DmrClusterName.Equals(input.DmrClusterName))
                ) && 
                (
                    this.EgressFlowWindowSize == input.EgressFlowWindowSize ||
                    (this.EgressFlowWindowSize != null &&
                    this.EgressFlowWindowSize.Equals(input.EgressFlowWindowSize))
                ) && 
                (
                    this.Enabled == input.Enabled ||
                    (this.Enabled != null &&
                    this.Enabled.Equals(input.Enabled))
                ) && 
                (
                    this.Initiator == input.Initiator ||
                    (this.Initiator != null &&
                    this.Initiator.Equals(input.Initiator))
                ) && 
                (
                    this.QueueDeadMsgQueue == input.QueueDeadMsgQueue ||
                    (this.QueueDeadMsgQueue != null &&
                    this.QueueDeadMsgQueue.Equals(input.QueueDeadMsgQueue))
                ) && 
                (
                    this.QueueEventSpoolUsageThreshold == input.QueueEventSpoolUsageThreshold ||
                    (this.QueueEventSpoolUsageThreshold != null &&
                    this.QueueEventSpoolUsageThreshold.Equals(input.QueueEventSpoolUsageThreshold))
                ) && 
                (
                    this.QueueMaxDeliveredUnackedMsgsPerFlow == input.QueueMaxDeliveredUnackedMsgsPerFlow ||
                    (this.QueueMaxDeliveredUnackedMsgsPerFlow != null &&
                    this.QueueMaxDeliveredUnackedMsgsPerFlow.Equals(input.QueueMaxDeliveredUnackedMsgsPerFlow))
                ) && 
                (
                    this.QueueMaxMsgSpoolUsage == input.QueueMaxMsgSpoolUsage ||
                    (this.QueueMaxMsgSpoolUsage != null &&
                    this.QueueMaxMsgSpoolUsage.Equals(input.QueueMaxMsgSpoolUsage))
                ) && 
                (
                    this.QueueMaxRedeliveryCount == input.QueueMaxRedeliveryCount ||
                    (this.QueueMaxRedeliveryCount != null &&
                    this.QueueMaxRedeliveryCount.Equals(input.QueueMaxRedeliveryCount))
                ) && 
                (
                    this.QueueMaxTtl == input.QueueMaxTtl ||
                    (this.QueueMaxTtl != null &&
                    this.QueueMaxTtl.Equals(input.QueueMaxTtl))
                ) && 
                (
                    this.QueueRejectMsgToSenderOnDiscardBehavior == input.QueueRejectMsgToSenderOnDiscardBehavior ||
                    (this.QueueRejectMsgToSenderOnDiscardBehavior != null &&
                    this.QueueRejectMsgToSenderOnDiscardBehavior.Equals(input.QueueRejectMsgToSenderOnDiscardBehavior))
                ) && 
                (
                    this.QueueRespectTtlEnabled == input.QueueRespectTtlEnabled ||
                    (this.QueueRespectTtlEnabled != null &&
                    this.QueueRespectTtlEnabled.Equals(input.QueueRespectTtlEnabled))
                ) && 
                (
                    this.RemoteNodeName == input.RemoteNodeName ||
                    (this.RemoteNodeName != null &&
                    this.RemoteNodeName.Equals(input.RemoteNodeName))
                ) && 
                (
                    this.Span == input.Span ||
                    (this.Span != null &&
                    this.Span.Equals(input.Span))
                ) && 
                (
                    this.TransportCompressedEnabled == input.TransportCompressedEnabled ||
                    (this.TransportCompressedEnabled != null &&
                    this.TransportCompressedEnabled.Equals(input.TransportCompressedEnabled))
                ) && 
                (
                    this.TransportTlsEnabled == input.TransportTlsEnabled ||
                    (this.TransportTlsEnabled != null &&
                    this.TransportTlsEnabled.Equals(input.TransportTlsEnabled))
                );
        }

        /// <summary>
        /// Gets the hash code
        /// </summary>
        /// <returns>Hash code</returns>
        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hashCode = 41;
                if (this.AuthenticationBasicPassword != null)
                    hashCode = hashCode * 59 + this.AuthenticationBasicPassword.GetHashCode();
                if (this.AuthenticationScheme != null)
                    hashCode = hashCode * 59 + this.AuthenticationScheme.GetHashCode();
                if (this.ClientProfileQueueControl1MaxDepth != null)
                    hashCode = hashCode * 59 + this.ClientProfileQueueControl1MaxDepth.GetHashCode();
                if (this.ClientProfileQueueControl1MinMsgBurst != null)
                    hashCode = hashCode * 59 + this.ClientProfileQueueControl1MinMsgBurst.GetHashCode();
                if (this.ClientProfileQueueDirect1MaxDepth != null)
                    hashCode = hashCode * 59 + this.ClientProfileQueueDirect1MaxDepth.GetHashCode();
                if (this.ClientProfileQueueDirect1MinMsgBurst != null)
                    hashCode = hashCode * 59 + this.ClientProfileQueueDirect1MinMsgBurst.GetHashCode();
                if (this.ClientProfileQueueDirect2MaxDepth != null)
                    hashCode = hashCode * 59 + this.ClientProfileQueueDirect2MaxDepth.GetHashCode();
                if (this.ClientProfileQueueDirect2MinMsgBurst != null)
                    hashCode = hashCode * 59 + this.ClientProfileQueueDirect2MinMsgBurst.GetHashCode();
                if (this.ClientProfileQueueDirect3MaxDepth != null)
                    hashCode = hashCode * 59 + this.ClientProfileQueueDirect3MaxDepth.GetHashCode();
                if (this.ClientProfileQueueDirect3MinMsgBurst != null)
                    hashCode = hashCode * 59 + this.ClientProfileQueueDirect3MinMsgBurst.GetHashCode();
                if (this.ClientProfileQueueGuaranteed1MaxDepth != null)
                    hashCode = hashCode * 59 + this.ClientProfileQueueGuaranteed1MaxDepth.GetHashCode();
                if (this.ClientProfileQueueGuaranteed1MinMsgBurst != null)
                    hashCode = hashCode * 59 + this.ClientProfileQueueGuaranteed1MinMsgBurst.GetHashCode();
                if (this.ClientProfileTcpCongestionWindowSize != null)
                    hashCode = hashCode * 59 + this.ClientProfileTcpCongestionWindowSize.GetHashCode();
                if (this.ClientProfileTcpKeepaliveCount != null)
                    hashCode = hashCode * 59 + this.ClientProfileTcpKeepaliveCount.GetHashCode();
                if (this.ClientProfileTcpKeepaliveIdleTime != null)
                    hashCode = hashCode * 59 + this.ClientProfileTcpKeepaliveIdleTime.GetHashCode();
                if (this.ClientProfileTcpKeepaliveInterval != null)
                    hashCode = hashCode * 59 + this.ClientProfileTcpKeepaliveInterval.GetHashCode();
                if (this.ClientProfileTcpMaxSegmentSize != null)
                    hashCode = hashCode * 59 + this.ClientProfileTcpMaxSegmentSize.GetHashCode();
                if (this.ClientProfileTcpMaxWindowSize != null)
                    hashCode = hashCode * 59 + this.ClientProfileTcpMaxWindowSize.GetHashCode();
                if (this.DmrClusterName != null)
                    hashCode = hashCode * 59 + this.DmrClusterName.GetHashCode();
                if (this.EgressFlowWindowSize != null)
                    hashCode = hashCode * 59 + this.EgressFlowWindowSize.GetHashCode();
                if (this.Enabled != null)
                    hashCode = hashCode * 59 + this.Enabled.GetHashCode();
                if (this.Initiator != null)
                    hashCode = hashCode * 59 + this.Initiator.GetHashCode();
                if (this.QueueDeadMsgQueue != null)
                    hashCode = hashCode * 59 + this.QueueDeadMsgQueue.GetHashCode();
                if (this.QueueEventSpoolUsageThreshold != null)
                    hashCode = hashCode * 59 + this.QueueEventSpoolUsageThreshold.GetHashCode();
                if (this.QueueMaxDeliveredUnackedMsgsPerFlow != null)
                    hashCode = hashCode * 59 + this.QueueMaxDeliveredUnackedMsgsPerFlow.GetHashCode();
                if (this.QueueMaxMsgSpoolUsage != null)
                    hashCode = hashCode * 59 + this.QueueMaxMsgSpoolUsage.GetHashCode();
                if (this.QueueMaxRedeliveryCount != null)
                    hashCode = hashCode * 59 + this.QueueMaxRedeliveryCount.GetHashCode();
                if (this.QueueMaxTtl != null)
                    hashCode = hashCode * 59 + this.QueueMaxTtl.GetHashCode();
                if (this.QueueRejectMsgToSenderOnDiscardBehavior != null)
                    hashCode = hashCode * 59 + this.QueueRejectMsgToSenderOnDiscardBehavior.GetHashCode();
                if (this.QueueRespectTtlEnabled != null)
                    hashCode = hashCode * 59 + this.QueueRespectTtlEnabled.GetHashCode();
                if (this.RemoteNodeName != null)
                    hashCode = hashCode * 59 + this.RemoteNodeName.GetHashCode();
                if (this.Span != null)
                    hashCode = hashCode * 59 + this.Span.GetHashCode();
                if (this.TransportCompressedEnabled != null)
                    hashCode = hashCode * 59 + this.TransportCompressedEnabled.GetHashCode();
                if (this.TransportTlsEnabled != null)
                    hashCode = hashCode * 59 + this.TransportTlsEnabled.GetHashCode();
                return hashCode;
            }
        }
    }
}
