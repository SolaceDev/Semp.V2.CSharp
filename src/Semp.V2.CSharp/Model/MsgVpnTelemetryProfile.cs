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
    /// MsgVpnTelemetryProfile
    /// </summary>
    [DataContract]
        public partial class MsgVpnTelemetryProfile :  IEquatable<MsgVpnTelemetryProfile>
    {
        /// <summary>
        /// The default action to take when a receiver client connects to the broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;disallow\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow\&quot; - Allow client connection unless an exception is found for it. \&quot;disallow\&quot; - Disallow client connection unless an exception is found for it. &lt;/pre&gt; 
        /// </summary>
        /// <value>The default action to take when a receiver client connects to the broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;disallow\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow\&quot; - Allow client connection unless an exception is found for it. \&quot;disallow\&quot; - Disallow client connection unless an exception is found for it. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum ReceiverAclConnectDefaultActionEnum
        {
            /// <summary>
            /// Enum Allow for value: allow
            /// </summary>
            [EnumMember(Value = "allow")]
            Allow = 1,
            /// <summary>
            /// Enum Disallow for value: disallow
            /// </summary>
            [EnumMember(Value = "disallow")]
            Disallow = 2        }
        /// <summary>
        /// The default action to take when a receiver client connects to the broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;disallow\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow\&quot; - Allow client connection unless an exception is found for it. \&quot;disallow\&quot; - Disallow client connection unless an exception is found for it. &lt;/pre&gt; 
        /// </summary>
        /// <value>The default action to take when a receiver client connects to the broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;disallow\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow\&quot; - Allow client connection unless an exception is found for it. \&quot;disallow\&quot; - Disallow client connection unless an exception is found for it. &lt;/pre&gt; </value>
        [DataMember(Name="receiverAclConnectDefaultAction", EmitDefaultValue=false)]
        public ReceiverAclConnectDefaultActionEnum? ReceiverAclConnectDefaultAction { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="MsgVpnTelemetryProfile" /> class.
        /// </summary>
        /// <param name="msgVpnName">The name of the Message VPN..</param>
        /// <param name="queueEventBindCountThreshold">queueEventBindCountThreshold.</param>
        /// <param name="queueEventMsgSpoolUsageThreshold">queueEventMsgSpoolUsageThreshold.</param>
        /// <param name="queueMaxBindCount">The maximum number of consumer flows that can bind to the Queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;..</param>
        /// <param name="queueMaxMsgSpoolUsage">The maximum message spool usage allowed by the Queue, in megabytes (MB). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;800000&#x60;..</param>
        /// <param name="receiverAclConnectDefaultAction">The default action to take when a receiver client connects to the broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;disallow\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow\&quot; - Allow client connection unless an exception is found for it. \&quot;disallow\&quot; - Disallow client connection unless an exception is found for it. &lt;/pre&gt; .</param>
        /// <param name="receiverEnabled">Enable or disable the ability for receiver clients to consume from the #telemetry queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="receiverEventConnectionCountPerClientUsernameThreshold">receiverEventConnectionCountPerClientUsernameThreshold.</param>
        /// <param name="receiverMaxConnectionCountPerClientUsername">The maximum number of receiver connections per Client Username. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform..</param>
        /// <param name="receiverTcpCongestionWindowSize">The TCP initial congestion window size for clients using the Client Profile, in multiples of the TCP Maximum Segment Size (MSS). Changing the value from its default of 2 results in non-compliance with RFC 2581. Contact support before changing this value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2&#x60;..</param>
        /// <param name="receiverTcpKeepaliveCount">The number of TCP keepalive retransmissions to a client using the Client Profile before declaring that it is not available. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;5&#x60;..</param>
        /// <param name="receiverTcpKeepaliveIdleTime">The amount of time a client connection using the Client Profile must remain idle before TCP begins sending keepalive probes, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;..</param>
        /// <param name="receiverTcpKeepaliveInterval">The amount of time between TCP keepalive retransmissions to a client using the Client Profile when no acknowledgement is received, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;..</param>
        /// <param name="receiverTcpMaxSegmentSize">The TCP maximum segment size for clients using the Client Profile, in bytes. Changes are applied to all existing connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1460&#x60;..</param>
        /// <param name="receiverTcpMaxWindowSize">The TCP maximum window size for clients using the Client Profile, in kilobytes. Changes are applied to all existing connections. This setting is ignored on the software broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;256&#x60;..</param>
        /// <param name="telemetryProfileName">The name of the Telemetry Profile..</param>
        /// <param name="traceEnabled">Enable or disable generation of all trace span data messages. When enabled, the state of configured trace filters control which messages get traced. When disabled, trace span data messages are never generated, regardless of the state of trace filters. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;..</param>
        public MsgVpnTelemetryProfile(string msgVpnName = default(string), EventThreshold queueEventBindCountThreshold = default(EventThreshold), EventThreshold queueEventMsgSpoolUsageThreshold = default(EventThreshold), long? queueMaxBindCount = default(long?), long? queueMaxMsgSpoolUsage = default(long?), ReceiverAclConnectDefaultActionEnum? receiverAclConnectDefaultAction = default(ReceiverAclConnectDefaultActionEnum?), bool? receiverEnabled = default(bool?), EventThreshold receiverEventConnectionCountPerClientUsernameThreshold = default(EventThreshold), long? receiverMaxConnectionCountPerClientUsername = default(long?), long? receiverTcpCongestionWindowSize = default(long?), long? receiverTcpKeepaliveCount = default(long?), long? receiverTcpKeepaliveIdleTime = default(long?), long? receiverTcpKeepaliveInterval = default(long?), long? receiverTcpMaxSegmentSize = default(long?), long? receiverTcpMaxWindowSize = default(long?), string telemetryProfileName = default(string), bool? traceEnabled = default(bool?))
        {
            this.MsgVpnName = msgVpnName;
            this.QueueEventBindCountThreshold = queueEventBindCountThreshold;
            this.QueueEventMsgSpoolUsageThreshold = queueEventMsgSpoolUsageThreshold;
            this.QueueMaxBindCount = queueMaxBindCount;
            this.QueueMaxMsgSpoolUsage = queueMaxMsgSpoolUsage;
            this.ReceiverAclConnectDefaultAction = receiverAclConnectDefaultAction;
            this.ReceiverEnabled = receiverEnabled;
            this.ReceiverEventConnectionCountPerClientUsernameThreshold = receiverEventConnectionCountPerClientUsernameThreshold;
            this.ReceiverMaxConnectionCountPerClientUsername = receiverMaxConnectionCountPerClientUsername;
            this.ReceiverTcpCongestionWindowSize = receiverTcpCongestionWindowSize;
            this.ReceiverTcpKeepaliveCount = receiverTcpKeepaliveCount;
            this.ReceiverTcpKeepaliveIdleTime = receiverTcpKeepaliveIdleTime;
            this.ReceiverTcpKeepaliveInterval = receiverTcpKeepaliveInterval;
            this.ReceiverTcpMaxSegmentSize = receiverTcpMaxSegmentSize;
            this.ReceiverTcpMaxWindowSize = receiverTcpMaxWindowSize;
            this.TelemetryProfileName = telemetryProfileName;
            this.TraceEnabled = traceEnabled;
        }
        
        /// <summary>
        /// The name of the Message VPN.
        /// </summary>
        /// <value>The name of the Message VPN.</value>
        [DataMember(Name="msgVpnName", EmitDefaultValue=false)]
        public string MsgVpnName { get; set; }

        /// <summary>
        /// Gets or Sets QueueEventBindCountThreshold
        /// </summary>
        [DataMember(Name="queueEventBindCountThreshold", EmitDefaultValue=false)]
        public EventThreshold QueueEventBindCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets QueueEventMsgSpoolUsageThreshold
        /// </summary>
        [DataMember(Name="queueEventMsgSpoolUsageThreshold", EmitDefaultValue=false)]
        public EventThreshold QueueEventMsgSpoolUsageThreshold { get; set; }

        /// <summary>
        /// The maximum number of consumer flows that can bind to the Queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.
        /// </summary>
        /// <value>The maximum number of consumer flows that can bind to the Queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;.</value>
        [DataMember(Name="queueMaxBindCount", EmitDefaultValue=false)]
        public long? QueueMaxBindCount { get; set; }

        /// <summary>
        /// The maximum message spool usage allowed by the Queue, in megabytes (MB). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;800000&#x60;.
        /// </summary>
        /// <value>The maximum message spool usage allowed by the Queue, in megabytes (MB). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;800000&#x60;.</value>
        [DataMember(Name="queueMaxMsgSpoolUsage", EmitDefaultValue=false)]
        public long? QueueMaxMsgSpoolUsage { get; set; }


        /// <summary>
        /// Enable or disable the ability for receiver clients to consume from the #telemetry queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the ability for receiver clients to consume from the #telemetry queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="receiverEnabled", EmitDefaultValue=false)]
        public bool? ReceiverEnabled { get; set; }

        /// <summary>
        /// Gets or Sets ReceiverEventConnectionCountPerClientUsernameThreshold
        /// </summary>
        [DataMember(Name="receiverEventConnectionCountPerClientUsernameThreshold", EmitDefaultValue=false)]
        public EventThreshold ReceiverEventConnectionCountPerClientUsernameThreshold { get; set; }

        /// <summary>
        /// The maximum number of receiver connections per Client Username. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.
        /// </summary>
        /// <value>The maximum number of receiver connections per Client Username. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform.</value>
        [DataMember(Name="receiverMaxConnectionCountPerClientUsername", EmitDefaultValue=false)]
        public long? ReceiverMaxConnectionCountPerClientUsername { get; set; }

        /// <summary>
        /// The TCP initial congestion window size for clients using the Client Profile, in multiples of the TCP Maximum Segment Size (MSS). Changing the value from its default of 2 results in non-compliance with RFC 2581. Contact support before changing this value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2&#x60;.
        /// </summary>
        /// <value>The TCP initial congestion window size for clients using the Client Profile, in multiples of the TCP Maximum Segment Size (MSS). Changing the value from its default of 2 results in non-compliance with RFC 2581. Contact support before changing this value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2&#x60;.</value>
        [DataMember(Name="receiverTcpCongestionWindowSize", EmitDefaultValue=false)]
        public long? ReceiverTcpCongestionWindowSize { get; set; }

        /// <summary>
        /// The number of TCP keepalive retransmissions to a client using the Client Profile before declaring that it is not available. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;5&#x60;.
        /// </summary>
        /// <value>The number of TCP keepalive retransmissions to a client using the Client Profile before declaring that it is not available. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;5&#x60;.</value>
        [DataMember(Name="receiverTcpKeepaliveCount", EmitDefaultValue=false)]
        public long? ReceiverTcpKeepaliveCount { get; set; }

        /// <summary>
        /// The amount of time a client connection using the Client Profile must remain idle before TCP begins sending keepalive probes, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.
        /// </summary>
        /// <value>The amount of time a client connection using the Client Profile must remain idle before TCP begins sending keepalive probes, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;.</value>
        [DataMember(Name="receiverTcpKeepaliveIdleTime", EmitDefaultValue=false)]
        public long? ReceiverTcpKeepaliveIdleTime { get; set; }

        /// <summary>
        /// The amount of time between TCP keepalive retransmissions to a client using the Client Profile when no acknowledgement is received, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;.
        /// </summary>
        /// <value>The amount of time between TCP keepalive retransmissions to a client using the Client Profile when no acknowledgement is received, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;.</value>
        [DataMember(Name="receiverTcpKeepaliveInterval", EmitDefaultValue=false)]
        public long? ReceiverTcpKeepaliveInterval { get; set; }

        /// <summary>
        /// The TCP maximum segment size for clients using the Client Profile, in bytes. Changes are applied to all existing connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1460&#x60;.
        /// </summary>
        /// <value>The TCP maximum segment size for clients using the Client Profile, in bytes. Changes are applied to all existing connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1460&#x60;.</value>
        [DataMember(Name="receiverTcpMaxSegmentSize", EmitDefaultValue=false)]
        public long? ReceiverTcpMaxSegmentSize { get; set; }

        /// <summary>
        /// The TCP maximum window size for clients using the Client Profile, in kilobytes. Changes are applied to all existing connections. This setting is ignored on the software broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;256&#x60;.
        /// </summary>
        /// <value>The TCP maximum window size for clients using the Client Profile, in kilobytes. Changes are applied to all existing connections. This setting is ignored on the software broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;256&#x60;.</value>
        [DataMember(Name="receiverTcpMaxWindowSize", EmitDefaultValue=false)]
        public long? ReceiverTcpMaxWindowSize { get; set; }

        /// <summary>
        /// The name of the Telemetry Profile.
        /// </summary>
        /// <value>The name of the Telemetry Profile.</value>
        [DataMember(Name="telemetryProfileName", EmitDefaultValue=false)]
        public string TelemetryProfileName { get; set; }

        /// <summary>
        /// Enable or disable generation of all trace span data messages. When enabled, the state of configured trace filters control which messages get traced. When disabled, trace span data messages are never generated, regardless of the state of trace filters. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable generation of all trace span data messages. When enabled, the state of configured trace filters control which messages get traced. When disabled, trace span data messages are never generated, regardless of the state of trace filters. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="traceEnabled", EmitDefaultValue=false)]
        public bool? TraceEnabled { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class MsgVpnTelemetryProfile {\n");
            sb.Append("  MsgVpnName: ").Append(MsgVpnName).Append("\n");
            sb.Append("  QueueEventBindCountThreshold: ").Append(QueueEventBindCountThreshold).Append("\n");
            sb.Append("  QueueEventMsgSpoolUsageThreshold: ").Append(QueueEventMsgSpoolUsageThreshold).Append("\n");
            sb.Append("  QueueMaxBindCount: ").Append(QueueMaxBindCount).Append("\n");
            sb.Append("  QueueMaxMsgSpoolUsage: ").Append(QueueMaxMsgSpoolUsage).Append("\n");
            sb.Append("  ReceiverAclConnectDefaultAction: ").Append(ReceiverAclConnectDefaultAction).Append("\n");
            sb.Append("  ReceiverEnabled: ").Append(ReceiverEnabled).Append("\n");
            sb.Append("  ReceiverEventConnectionCountPerClientUsernameThreshold: ").Append(ReceiverEventConnectionCountPerClientUsernameThreshold).Append("\n");
            sb.Append("  ReceiverMaxConnectionCountPerClientUsername: ").Append(ReceiverMaxConnectionCountPerClientUsername).Append("\n");
            sb.Append("  ReceiverTcpCongestionWindowSize: ").Append(ReceiverTcpCongestionWindowSize).Append("\n");
            sb.Append("  ReceiverTcpKeepaliveCount: ").Append(ReceiverTcpKeepaliveCount).Append("\n");
            sb.Append("  ReceiverTcpKeepaliveIdleTime: ").Append(ReceiverTcpKeepaliveIdleTime).Append("\n");
            sb.Append("  ReceiverTcpKeepaliveInterval: ").Append(ReceiverTcpKeepaliveInterval).Append("\n");
            sb.Append("  ReceiverTcpMaxSegmentSize: ").Append(ReceiverTcpMaxSegmentSize).Append("\n");
            sb.Append("  ReceiverTcpMaxWindowSize: ").Append(ReceiverTcpMaxWindowSize).Append("\n");
            sb.Append("  TelemetryProfileName: ").Append(TelemetryProfileName).Append("\n");
            sb.Append("  TraceEnabled: ").Append(TraceEnabled).Append("\n");
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
            return this.Equals(input as MsgVpnTelemetryProfile);
        }

        /// <summary>
        /// Returns true if MsgVpnTelemetryProfile instances are equal
        /// </summary>
        /// <param name="input">Instance of MsgVpnTelemetryProfile to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(MsgVpnTelemetryProfile input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.MsgVpnName == input.MsgVpnName ||
                    (this.MsgVpnName != null &&
                    this.MsgVpnName.Equals(input.MsgVpnName))
                ) && 
                (
                    this.QueueEventBindCountThreshold == input.QueueEventBindCountThreshold ||
                    (this.QueueEventBindCountThreshold != null &&
                    this.QueueEventBindCountThreshold.Equals(input.QueueEventBindCountThreshold))
                ) && 
                (
                    this.QueueEventMsgSpoolUsageThreshold == input.QueueEventMsgSpoolUsageThreshold ||
                    (this.QueueEventMsgSpoolUsageThreshold != null &&
                    this.QueueEventMsgSpoolUsageThreshold.Equals(input.QueueEventMsgSpoolUsageThreshold))
                ) && 
                (
                    this.QueueMaxBindCount == input.QueueMaxBindCount ||
                    (this.QueueMaxBindCount != null &&
                    this.QueueMaxBindCount.Equals(input.QueueMaxBindCount))
                ) && 
                (
                    this.QueueMaxMsgSpoolUsage == input.QueueMaxMsgSpoolUsage ||
                    (this.QueueMaxMsgSpoolUsage != null &&
                    this.QueueMaxMsgSpoolUsage.Equals(input.QueueMaxMsgSpoolUsage))
                ) && 
                (
                    this.ReceiverAclConnectDefaultAction == input.ReceiverAclConnectDefaultAction ||
                    (this.ReceiverAclConnectDefaultAction != null &&
                    this.ReceiverAclConnectDefaultAction.Equals(input.ReceiverAclConnectDefaultAction))
                ) && 
                (
                    this.ReceiverEnabled == input.ReceiverEnabled ||
                    (this.ReceiverEnabled != null &&
                    this.ReceiverEnabled.Equals(input.ReceiverEnabled))
                ) && 
                (
                    this.ReceiverEventConnectionCountPerClientUsernameThreshold == input.ReceiverEventConnectionCountPerClientUsernameThreshold ||
                    (this.ReceiverEventConnectionCountPerClientUsernameThreshold != null &&
                    this.ReceiverEventConnectionCountPerClientUsernameThreshold.Equals(input.ReceiverEventConnectionCountPerClientUsernameThreshold))
                ) && 
                (
                    this.ReceiverMaxConnectionCountPerClientUsername == input.ReceiverMaxConnectionCountPerClientUsername ||
                    (this.ReceiverMaxConnectionCountPerClientUsername != null &&
                    this.ReceiverMaxConnectionCountPerClientUsername.Equals(input.ReceiverMaxConnectionCountPerClientUsername))
                ) && 
                (
                    this.ReceiverTcpCongestionWindowSize == input.ReceiverTcpCongestionWindowSize ||
                    (this.ReceiverTcpCongestionWindowSize != null &&
                    this.ReceiverTcpCongestionWindowSize.Equals(input.ReceiverTcpCongestionWindowSize))
                ) && 
                (
                    this.ReceiverTcpKeepaliveCount == input.ReceiverTcpKeepaliveCount ||
                    (this.ReceiverTcpKeepaliveCount != null &&
                    this.ReceiverTcpKeepaliveCount.Equals(input.ReceiverTcpKeepaliveCount))
                ) && 
                (
                    this.ReceiverTcpKeepaliveIdleTime == input.ReceiverTcpKeepaliveIdleTime ||
                    (this.ReceiverTcpKeepaliveIdleTime != null &&
                    this.ReceiverTcpKeepaliveIdleTime.Equals(input.ReceiverTcpKeepaliveIdleTime))
                ) && 
                (
                    this.ReceiverTcpKeepaliveInterval == input.ReceiverTcpKeepaliveInterval ||
                    (this.ReceiverTcpKeepaliveInterval != null &&
                    this.ReceiverTcpKeepaliveInterval.Equals(input.ReceiverTcpKeepaliveInterval))
                ) && 
                (
                    this.ReceiverTcpMaxSegmentSize == input.ReceiverTcpMaxSegmentSize ||
                    (this.ReceiverTcpMaxSegmentSize != null &&
                    this.ReceiverTcpMaxSegmentSize.Equals(input.ReceiverTcpMaxSegmentSize))
                ) && 
                (
                    this.ReceiverTcpMaxWindowSize == input.ReceiverTcpMaxWindowSize ||
                    (this.ReceiverTcpMaxWindowSize != null &&
                    this.ReceiverTcpMaxWindowSize.Equals(input.ReceiverTcpMaxWindowSize))
                ) && 
                (
                    this.TelemetryProfileName == input.TelemetryProfileName ||
                    (this.TelemetryProfileName != null &&
                    this.TelemetryProfileName.Equals(input.TelemetryProfileName))
                ) && 
                (
                    this.TraceEnabled == input.TraceEnabled ||
                    (this.TraceEnabled != null &&
                    this.TraceEnabled.Equals(input.TraceEnabled))
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
                if (this.MsgVpnName != null)
                    hashCode = hashCode * 59 + this.MsgVpnName.GetHashCode();
                if (this.QueueEventBindCountThreshold != null)
                    hashCode = hashCode * 59 + this.QueueEventBindCountThreshold.GetHashCode();
                if (this.QueueEventMsgSpoolUsageThreshold != null)
                    hashCode = hashCode * 59 + this.QueueEventMsgSpoolUsageThreshold.GetHashCode();
                if (this.QueueMaxBindCount != null)
                    hashCode = hashCode * 59 + this.QueueMaxBindCount.GetHashCode();
                if (this.QueueMaxMsgSpoolUsage != null)
                    hashCode = hashCode * 59 + this.QueueMaxMsgSpoolUsage.GetHashCode();
                if (this.ReceiverAclConnectDefaultAction != null)
                    hashCode = hashCode * 59 + this.ReceiverAclConnectDefaultAction.GetHashCode();
                if (this.ReceiverEnabled != null)
                    hashCode = hashCode * 59 + this.ReceiverEnabled.GetHashCode();
                if (this.ReceiverEventConnectionCountPerClientUsernameThreshold != null)
                    hashCode = hashCode * 59 + this.ReceiverEventConnectionCountPerClientUsernameThreshold.GetHashCode();
                if (this.ReceiverMaxConnectionCountPerClientUsername != null)
                    hashCode = hashCode * 59 + this.ReceiverMaxConnectionCountPerClientUsername.GetHashCode();
                if (this.ReceiverTcpCongestionWindowSize != null)
                    hashCode = hashCode * 59 + this.ReceiverTcpCongestionWindowSize.GetHashCode();
                if (this.ReceiverTcpKeepaliveCount != null)
                    hashCode = hashCode * 59 + this.ReceiverTcpKeepaliveCount.GetHashCode();
                if (this.ReceiverTcpKeepaliveIdleTime != null)
                    hashCode = hashCode * 59 + this.ReceiverTcpKeepaliveIdleTime.GetHashCode();
                if (this.ReceiverTcpKeepaliveInterval != null)
                    hashCode = hashCode * 59 + this.ReceiverTcpKeepaliveInterval.GetHashCode();
                if (this.ReceiverTcpMaxSegmentSize != null)
                    hashCode = hashCode * 59 + this.ReceiverTcpMaxSegmentSize.GetHashCode();
                if (this.ReceiverTcpMaxWindowSize != null)
                    hashCode = hashCode * 59 + this.ReceiverTcpMaxWindowSize.GetHashCode();
                if (this.TelemetryProfileName != null)
                    hashCode = hashCode * 59 + this.TelemetryProfileName.GetHashCode();
                if (this.TraceEnabled != null)
                    hashCode = hashCode * 59 + this.TraceEnabled.GetHashCode();
                return hashCode;
            }
        }
    }
}
