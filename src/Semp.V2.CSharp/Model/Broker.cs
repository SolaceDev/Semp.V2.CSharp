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
    /// Broker
    /// </summary>
    [DataContract]
        public partial class Broker :  IEquatable<Broker>
    {
        /// <summary>
        /// The client certificate revocation checking mode used when a client authenticates with a client certificate. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - Do not perform any certificate revocation checking. \&quot;ocsp\&quot; - Use the Open Certificate Status Protcol (OCSP) for certificate revocation checking. \&quot;crl\&quot; - Use Certificate Revocation Lists (CRL) for certificate revocation checking. \&quot;ocsp-crl\&quot; - Use OCSP first, but if OCSP fails to return an unambiguous result, then check via CRL. &lt;/pre&gt; 
        /// </summary>
        /// <value>The client certificate revocation checking mode used when a client authenticates with a client certificate. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - Do not perform any certificate revocation checking. \&quot;ocsp\&quot; - Use the Open Certificate Status Protcol (OCSP) for certificate revocation checking. \&quot;crl\&quot; - Use Certificate Revocation Lists (CRL) for certificate revocation checking. \&quot;ocsp-crl\&quot; - Use OCSP first, but if OCSP fails to return an unambiguous result, then check via CRL. &lt;/pre&gt; </value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum AuthClientCertRevocationCheckModeEnum
        {
            /// <summary>
            /// Enum None for value: none
            /// </summary>
            [EnumMember(Value = "none")]
            None = 1,
            /// <summary>
            /// Enum Ocsp for value: ocsp
            /// </summary>
            [EnumMember(Value = "ocsp")]
            Ocsp = 2,
            /// <summary>
            /// Enum Crl for value: crl
            /// </summary>
            [EnumMember(Value = "crl")]
            Crl = 3,
            /// <summary>
            /// Enum OcspCrl for value: ocsp-crl
            /// </summary>
            [EnumMember(Value = "ocsp-crl")]
            OcspCrl = 4        }
        /// <summary>
        /// The client certificate revocation checking mode used when a client authenticates with a client certificate. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - Do not perform any certificate revocation checking. \&quot;ocsp\&quot; - Use the Open Certificate Status Protcol (OCSP) for certificate revocation checking. \&quot;crl\&quot; - Use Certificate Revocation Lists (CRL) for certificate revocation checking. \&quot;ocsp-crl\&quot; - Use OCSP first, but if OCSP fails to return an unambiguous result, then check via CRL. &lt;/pre&gt; 
        /// </summary>
        /// <value>The client certificate revocation checking mode used when a client authenticates with a client certificate. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - Do not perform any certificate revocation checking. \&quot;ocsp\&quot; - Use the Open Certificate Status Protcol (OCSP) for certificate revocation checking. \&quot;crl\&quot; - Use Certificate Revocation Lists (CRL) for certificate revocation checking. \&quot;ocsp-crl\&quot; - Use OCSP first, but if OCSP fails to return an unambiguous result, then check via CRL. &lt;/pre&gt; </value>
        [DataMember(Name="authClientCertRevocationCheckMode", EmitDefaultValue=false)]
        public AuthClientCertRevocationCheckModeEnum? AuthClientCertRevocationCheckMode { get; set; }
        /// <summary>
        /// The replication compatibility mode for the router. The default value is &#x60;\&quot;legacy\&quot;&#x60;. The allowed values and their meaning are:\&quot;legacy\&quot; - All transactions originated by clients are replicated to the standby site without using transactions.\&quot;transacted\&quot; - All transactions originated by clients are replicated to the standby site using transactions. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;legacy\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;legacy\&quot; - All transactions originated by clients are replicated to the standby site without using transactions. \&quot;transacted\&quot; - All transactions originated by clients are replicated to the standby site using transactions. &lt;/pre&gt;  Available since 2.18.
        /// </summary>
        /// <value>The replication compatibility mode for the router. The default value is &#x60;\&quot;legacy\&quot;&#x60;. The allowed values and their meaning are:\&quot;legacy\&quot; - All transactions originated by clients are replicated to the standby site without using transactions.\&quot;transacted\&quot; - All transactions originated by clients are replicated to the standby site using transactions. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;legacy\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;legacy\&quot; - All transactions originated by clients are replicated to the standby site without using transactions. \&quot;transacted\&quot; - All transactions originated by clients are replicated to the standby site using transactions. &lt;/pre&gt;  Available since 2.18.</value>
        [JsonConverter(typeof(StringEnumConverter))]
                public enum GuaranteedMsgingTransactionReplicationCompatibilityModeEnum
        {
            /// <summary>
            /// Enum Legacy for value: legacy
            /// </summary>
            [EnumMember(Value = "legacy")]
            Legacy = 1,
            /// <summary>
            /// Enum Transacted for value: transacted
            /// </summary>
            [EnumMember(Value = "transacted")]
            Transacted = 2        }
        /// <summary>
        /// The replication compatibility mode for the router. The default value is &#x60;\&quot;legacy\&quot;&#x60;. The allowed values and their meaning are:\&quot;legacy\&quot; - All transactions originated by clients are replicated to the standby site without using transactions.\&quot;transacted\&quot; - All transactions originated by clients are replicated to the standby site using transactions. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;legacy\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;legacy\&quot; - All transactions originated by clients are replicated to the standby site without using transactions. \&quot;transacted\&quot; - All transactions originated by clients are replicated to the standby site using transactions. &lt;/pre&gt;  Available since 2.18.
        /// </summary>
        /// <value>The replication compatibility mode for the router. The default value is &#x60;\&quot;legacy\&quot;&#x60;. The allowed values and their meaning are:\&quot;legacy\&quot; - All transactions originated by clients are replicated to the standby site without using transactions.\&quot;transacted\&quot; - All transactions originated by clients are replicated to the standby site using transactions. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;legacy\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;legacy\&quot; - All transactions originated by clients are replicated to the standby site without using transactions. \&quot;transacted\&quot; - All transactions originated by clients are replicated to the standby site using transactions. &lt;/pre&gt;  Available since 2.18.</value>
        [DataMember(Name="guaranteedMsgingTransactionReplicationCompatibilityMode", EmitDefaultValue=false)]
        public GuaranteedMsgingTransactionReplicationCompatibilityModeEnum? GuaranteedMsgingTransactionReplicationCompatibilityMode { get; set; }
        /// <summary>
        /// Initializes a new instance of the <see cref="Broker" /> class.
        /// </summary>
        /// <param name="authClientCertRevocationCheckMode">The client certificate revocation checking mode used when a client authenticates with a client certificate. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;none\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;none\&quot; - Do not perform any certificate revocation checking. \&quot;ocsp\&quot; - Use the Open Certificate Status Protcol (OCSP) for certificate revocation checking. \&quot;crl\&quot; - Use Certificate Revocation Lists (CRL) for certificate revocation checking. \&quot;ocsp-crl\&quot; - Use OCSP first, but if OCSP fails to return an unambiguous result, then check via CRL. &lt;/pre&gt; .</param>
        /// <param name="configSyncAuthenticationClientCertMaxChainDepth">The maximum depth for a client certificate chain. The depth of a chain is defined as the number of signing CA certificates that are present in the chain back to a trusted self-signed root CA certificate. The default value is &#x60;3&#x60;. Available since 2.22..</param>
        /// <param name="configSyncAuthenticationClientCertValidateDateEnabled">Enable or disable validation of the \&quot;Not Before\&quot; and \&quot;Not After\&quot; validity dates in the authentication certificate(s). The default value is &#x60;true&#x60;. Available since 2.22..</param>
        /// <param name="configSyncClientProfileTcpInitialCongestionWindow">The TCP initial congestion window size for Config Sync clients, in multiples of the TCP Maximum Segment Size (MSS). Changing the value from its default of 2 results in non-compliance with RFC 2581. Contact support before changing this value. The default value is &#x60;2&#x60;. Available since 2.22..</param>
        /// <param name="configSyncClientProfileTcpKeepaliveCount">The number of TCP keepalive retransmissions to a client using the Client Profile before declaring that it is not available. The default value is &#x60;5&#x60;. Available since 2.22..</param>
        /// <param name="configSyncClientProfileTcpKeepaliveIdle">The amount of time a client connection using the Client Profile must remain idle before TCP begins sending keepalive probes, in seconds. The default value is &#x60;3&#x60;. Available since 2.22..</param>
        /// <param name="configSyncClientProfileTcpKeepaliveInterval">The amount of time between TCP keepalive retransmissions to a client using the Client Profile when no acknowledgement is received, in seconds. The default value is &#x60;1&#x60;. Available since 2.22..</param>
        /// <param name="configSyncClientProfileTcpMaxWindow">The TCP maximum window size for clients using the Client Profile, in kilobytes. Changes are applied to all existing connections. This setting is ignored on the software broker. The default value is &#x60;256&#x60;. Available since 2.22..</param>
        /// <param name="configSyncClientProfileTcpMss">The TCP maximum segment size for clients using the Client Profile, in bytes. Changes are applied to all existing connections. The default value is &#x60;1460&#x60;. Available since 2.22..</param>
        /// <param name="configSyncEnabled">Enable or disable configuration synchronization for High Availability or Disaster Recovery. The default value is &#x60;false&#x60;. Available since 2.22..</param>
        /// <param name="configSyncSynchronizeUsernameEnabled">Enable or disable the synchronizing of usernames within High Availability groups. The transition from not synchronizing to synchronizing will cause the High Availability mate to fall out of sync. Recommendation: leave this as enabled. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.22..</param>
        /// <param name="configSyncTlsEnabled">Enable or disable the use of TLS encryption of the configuration synchronization communications between brokers in High Availability groups and/or Disaster Recovery sites. The default value is &#x60;false&#x60;. Available since 2.22..</param>
        /// <param name="guaranteedMsgingDefragmentationScheduleDayList">The days of the week to schedule defragmentation runs, specified as \&quot;daily\&quot; or as a comma-separated list of days. Days must be specified as \&quot;Sun\&quot;, \&quot;Mon\&quot;, \&quot;Tue\&quot;, \&quot;Wed\&quot;, \&quot;Thu\&quot;, \&quot;Fri, or \&quot;Sat\&quot;, with no spaces, and in sorted order from Sunday to Saturday. Please note \&quot;Sun,Mon,Tue,Wed,Thu,Fri,Sat\&quot; is not allowed, use \&quot;daily\&quot; instead. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;daily\&quot;&#x60;. Available since 2.25..</param>
        /// <param name="guaranteedMsgingDefragmentationScheduleEnabled">Enable or disable schedule-based defragmentation of Guaranteed Messaging spool files. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.25..</param>
        /// <param name="guaranteedMsgingDefragmentationScheduleTimeList">The times of the day to schedule defragmentation runs, specified as \&quot;hourly\&quot; or as a comma-separated list of 24-hour times in the form hh:mm, or h:mm. There must be no spaces, and times (up to 4) must be in sorted order from 0:00 to 23:59. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;0:00\&quot;&#x60;. Available since 2.25..</param>
        /// <param name="guaranteedMsgingDefragmentationThresholdEnabled">Enable or disable threshold-based defragmentation of Guaranteed Messaging spool files. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.25..</param>
        /// <param name="guaranteedMsgingDefragmentationThresholdFragmentationPercentage">Percentage of spool fragmentation needed to trigger defragmentation run. The minimum value allowed is 30%. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;50&#x60;. Available since 2.25..</param>
        /// <param name="guaranteedMsgingDefragmentationThresholdMinInterval">Minimum interval of time (in minutes) between defragmentation runs triggered by thresholds. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;15&#x60;. Available since 2.25..</param>
        /// <param name="guaranteedMsgingDefragmentationThresholdUsagePercentage">Percentage of spool usage needed to trigger defragmentation run. The minimum value allowed is 30%. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;50&#x60;. Available since 2.25..</param>
        /// <param name="guaranteedMsgingEnabled">Enable or disable Guaranteed Messaging. The default value is &#x60;false&#x60;. Available since 2.18..</param>
        /// <param name="guaranteedMsgingEventCacheUsageThreshold">guaranteedMsgingEventCacheUsageThreshold.</param>
        /// <param name="guaranteedMsgingEventDeliveredUnackedThreshold">guaranteedMsgingEventDeliveredUnackedThreshold.</param>
        /// <param name="guaranteedMsgingEventDiskUsageThreshold">guaranteedMsgingEventDiskUsageThreshold.</param>
        /// <param name="guaranteedMsgingEventEgressFlowCountThreshold">guaranteedMsgingEventEgressFlowCountThreshold.</param>
        /// <param name="guaranteedMsgingEventEndpointCountThreshold">guaranteedMsgingEventEndpointCountThreshold.</param>
        /// <param name="guaranteedMsgingEventIngressFlowCountThreshold">guaranteedMsgingEventIngressFlowCountThreshold.</param>
        /// <param name="guaranteedMsgingEventMsgCountThreshold">guaranteedMsgingEventMsgCountThreshold.</param>
        /// <param name="guaranteedMsgingEventMsgSpoolFileCountThreshold">guaranteedMsgingEventMsgSpoolFileCountThreshold.</param>
        /// <param name="guaranteedMsgingEventMsgSpoolUsageThreshold">guaranteedMsgingEventMsgSpoolUsageThreshold.</param>
        /// <param name="guaranteedMsgingEventTransactedSessionCountThreshold">guaranteedMsgingEventTransactedSessionCountThreshold.</param>
        /// <param name="guaranteedMsgingEventTransactedSessionResourceCountThreshold">guaranteedMsgingEventTransactedSessionResourceCountThreshold.</param>
        /// <param name="guaranteedMsgingEventTransactionCountThreshold">guaranteedMsgingEventTransactionCountThreshold.</param>
        /// <param name="guaranteedMsgingMaxCacheUsage">Guaranteed messaging cache usage limit. Expressed as a maximum percentage of the NAB&#x27;s egress queueing. resources that the guaranteed message cache is allowed to use. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;10&#x60;. Available since 2.18..</param>
        /// <param name="guaranteedMsgingMaxMsgSpoolUsage">The maximum total message spool usage allowed across all VPNs on this broker, in megabytes. Recommendation: the maximum value should be less than 90% of the disk space allocated for the guaranteed message spool. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1500&#x60;. Available since 2.18..</param>
        /// <param name="guaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout">The maximum time, in milliseconds, that can be tolerated for remote acknowledgement of synchronization messages before which the remote system will be considered out of sync. The default value is &#x60;10000&#x60;. Available since 2.18..</param>
        /// <param name="guaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout">The maximum time, in milliseconds, that can be tolerated for remote disk writes before which the remote system will be considered out of sync. The default value is &#x60;10000&#x60;. Available since 2.18..</param>
        /// <param name="guaranteedMsgingTransactionReplicationCompatibilityMode">The replication compatibility mode for the router. The default value is &#x60;\&quot;legacy\&quot;&#x60;. The allowed values and their meaning are:\&quot;legacy\&quot; - All transactions originated by clients are replicated to the standby site without using transactions.\&quot;transacted\&quot; - All transactions originated by clients are replicated to the standby site using transactions. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;legacy\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;legacy\&quot; - All transactions originated by clients are replicated to the standby site without using transactions. \&quot;transacted\&quot; - All transactions originated by clients are replicated to the standby site using transactions. &lt;/pre&gt;  Available since 2.18..</param>
        /// <param name="oauthProfileDefault">The default OAuth profile for OAuth authenticated SEMP requests. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.24..</param>
        /// <param name="serviceAmqpEnabled">Enable or disable the AMQP service. When disabled new AMQP Clients may not connect through the global or per-VPN AMQP listen-ports, and all currently connected AMQP Clients are immediately disconnected. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17..</param>
        /// <param name="serviceAmqpTlsListenPort">TCP port number that AMQP clients can use to connect to the broker using raw TCP over TLS. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceAmqpEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;. Available since 2.17..</param>
        /// <param name="serviceEventConnectionCountThreshold">serviceEventConnectionCountThreshold.</param>
        /// <param name="serviceHealthCheckEnabled">Enable or disable the plain-text health-check service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17..</param>
        /// <param name="serviceHealthCheckListenPort">The port number for the plain-text health-check service. The port must be unique across the message backbone. The health-check service must be disabled to change the port. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceHealthCheckEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;5550&#x60;. Available since 2.17..</param>
        /// <param name="serviceHealthCheckTlsEnabled">Enable or disable the TLS health-check service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.34..</param>
        /// <param name="serviceHealthCheckTlsListenPort">The port number for the TLS health-check service. The port must be unique across the message backbone. The health-check service must be disabled to change the port. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceHealthCheckTlsEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;. Available since 2.34..</param>
        /// <param name="serviceMateLinkEnabled">Enable or disable the mate-link service. The default value is &#x60;true&#x60;. Available since 2.17..</param>
        /// <param name="serviceMateLinkListenPort">The port number for the mate-link service. The port must be unique across the message backbone. The mate-link service must be disabled to change the port. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMateLinkEnabled will be temporarily set to false to apply the change. The default value is &#x60;8741&#x60;. Available since 2.17..</param>
        /// <param name="serviceMqttEnabled">Enable or disable the MQTT service. When disabled new MQTT Clients may not connect through the per-VPN MQTT listen-ports, and all currently connected MQTT Clients are immediately disconnected. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17..</param>
        /// <param name="serviceMsgBackboneEnabled">Enable or disable the msg-backbone service. When disabled new Clients may not connect through global or per-VPN listen-ports, and all currently connected Clients are immediately disconnected. The default value is &#x60;true&#x60;. Available since 2.17..</param>
        /// <param name="serviceRedundancyEnabled">Enable or disable the redundancy service. The default value is &#x60;true&#x60;. Available since 2.17..</param>
        /// <param name="serviceRedundancyFirstListenPort">The first listen-port used for the redundancy service. Redundancy uses this port and the subsequent 2 ports. These port must be unique across the message backbone. The redundancy service must be disabled to change this port. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceRedundancyEnabled will be temporarily set to false to apply the change. The default value is &#x60;8300&#x60;. Available since 2.17..</param>
        /// <param name="serviceRestEventOutgoingConnectionCountThreshold">serviceRestEventOutgoingConnectionCountThreshold.</param>
        /// <param name="serviceRestIncomingEnabled">Enable or disable the REST service incoming connections on the router. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17..</param>
        /// <param name="serviceRestOutgoingEnabled">Enable or disable the REST service outgoing connections on the router. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17..</param>
        /// <param name="serviceSempCorsAllowAnyHostEnabled">Enable or disable cross origin resource requests for the SEMP service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.24..</param>
        /// <param name="serviceSempLegacyTimeoutEnabled">Enable or disable extended SEMP timeouts for paged GETs. When a request times out, it returns the current page of content, even if the page is not full.  When enabled, the timeout is 60 seconds. When disabled, the timeout is 5 seconds.  The recommended setting is disabled (no legacy-timeout).  This parameter is intended as a temporary workaround to be used until SEMP clients can handle short pages.  This setting will be removed in a future release. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.18..</param>
        /// <param name="serviceSempPlainTextEnabled">Enable or disable plain-text SEMP service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.17..</param>
        /// <param name="serviceSempPlainTextListenPort">The TCP port for plain-text SEMP client connections. This attribute cannot be cannot be changed while serviceSempPlainTextEnabled are set to true. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;80&#x60;. Available since 2.17..</param>
        /// <param name="serviceSempSessionIdleTimeout">The session idle timeout, in minutes. Sessions will be invalidated if there is no activity in this period of time. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;15&#x60;. Available since 2.21..</param>
        /// <param name="serviceSempSessionMaxLifetime">The maximum lifetime of a session, in minutes. Sessions will be invalidated after this period of time, regardless of activity. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;43200&#x60;. Available since 2.21..</param>
        /// <param name="serviceSempTlsEnabled">Enable or disable TLS SEMP service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.17..</param>
        /// <param name="serviceSempTlsListenPort">The TCP port for TLS SEMP client connections. This attribute cannot be cannot be changed while serviceSempTlsEnabled are set to true. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1943&#x60;. Available since 2.17..</param>
        /// <param name="serviceSmfCompressionListenPort">TCP port number that SMF clients can use to connect to the broker using raw compression TCP. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceSmfEnabled will be temporarily set to false to apply the change. The default value is &#x60;55003&#x60;. Available since 2.17..</param>
        /// <param name="serviceSmfEnabled">Enable or disable the SMF service. When disabled new SMF Clients may not connect through the global listen-ports, and all currently connected SMF Clients are immediately disconnected. The default value is &#x60;true&#x60;. Available since 2.17..</param>
        /// <param name="serviceSmfEventConnectionCountThreshold">serviceSmfEventConnectionCountThreshold.</param>
        /// <param name="serviceSmfPlainTextListenPort">TCP port number that SMF clients can use to connect to the broker using raw TCP. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceSmfEnabled will be temporarily set to false to apply the change. The default value is &#x60;55555&#x60;. Available since 2.17..</param>
        /// <param name="serviceSmfRoutingControlListenPort">TCP port number that SMF clients can use to connect to the broker using raw routing control TCP. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceSmfEnabled will be temporarily set to false to apply the change. The default value is &#x60;55556&#x60;. Available since 2.17..</param>
        /// <param name="serviceSmfTlsListenPort">TCP port number that SMF clients can use to connect to the broker using raw TCP over TLS. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceSmfEnabled will be temporarily set to false to apply the change. The default value is &#x60;55443&#x60;. Available since 2.17..</param>
        /// <param name="serviceTlsEventConnectionCountThreshold">serviceTlsEventConnectionCountThreshold.</param>
        /// <param name="serviceWebTransportEnabled">Enable or disable the web-transport service. When disabled new web-transport Clients may not connect through the global listen-ports, and all currently connected web-transport Clients are immediately disconnected. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17..</param>
        /// <param name="serviceWebTransportPlainTextListenPort">The TCP port for plain-text WEB client connections. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceWebTransportEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;8008&#x60;. Available since 2.17..</param>
        /// <param name="serviceWebTransportTlsListenPort">The TCP port for TLS WEB client connections. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceWebTransportEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1443&#x60;. Available since 2.17..</param>
        /// <param name="serviceWebTransportWebUrlSuffix">Used to specify the Web URL suffix that will be used by Web clients when communicating with the broker. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceWebTransportEnabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.17..</param>
        /// <param name="tlsBlockVersion11Enabled">Enable or disable the blocking of TLS version 1.1 connections. When blocked, all existing incoming and outgoing TLS 1.1 connections with Clients, SEMP users, and LDAP servers remain connected while new connections are blocked. Note that support for TLS 1.1 will eventually be discontinued, at which time TLS 1.1 connections will be blocked regardless of this setting. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;..</param>
        /// <param name="tlsCipherSuiteManagementList">The colon-separated list of cipher suites used for TLS management connections (e.g. SEMP, LDAP). The value \&quot;default\&quot; implies all supported suites ordered from most secure to least secure. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;..</param>
        /// <param name="tlsCipherSuiteMsgBackboneList">The colon-separated list of cipher suites used for TLS data connections (e.g. client pub/sub). The value \&quot;default\&quot; implies all supported suites ordered from most secure to least secure. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;..</param>
        /// <param name="tlsCipherSuiteSecureShellList">The colon-separated list of cipher suites used for TLS secure shell connections (e.g. SSH, SFTP, SCP). The value \&quot;default\&quot; implies all supported suites ordered from most secure to least secure. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;..</param>
        /// <param name="tlsCrimeExploitProtectionEnabled">Enable or disable protection against the CRIME exploit. When enabled, TLS+compressed messaging performance is degraded. This protection should only be disabled if sufficient ACL and authentication features are being employed such that a potential attacker does not have sufficient access to trigger the exploit. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;..</param>
        /// <param name="tlsServerCertContent">The PEM formatted content for the server certificate used for TLS connections. It must consist of a private key and between one and three certificates comprising the certificate trust chain. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="tlsServerCertPassword">The password for the server certificate used for TLS connections. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. The default value is &#x60;\&quot;\&quot;&#x60;..</param>
        /// <param name="tlsStandardDomainCertificateAuthoritiesEnabled">Enable or disable the standard domain certificate authority list. The default value is &#x60;true&#x60;. Available since 2.19..</param>
        /// <param name="tlsTicketLifetime">The TLS ticket lifetime in seconds. When a client connects with TLS, a session with a session ticket is created using the TLS ticket lifetime which determines how long the client has to resume the session. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;86400&#x60;..</param>
        /// <param name="webManagerAllowUnencryptedWizardsEnabled">Enable or disable the use of unencrypted wizards in the Web-based Manager UI. This setting should be left at its default on all production systems or other systems that need to be secure.  Enabling this option will permit the broker to forward plain-text data to other brokers, making important information or credentials available for snooping. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.28..</param>
        /// <param name="webManagerCustomization">Reserved for internal use by Solace. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.25..</param>
        /// <param name="webManagerRedirectHttpEnabled">Enable or disable redirection of HTTP requests for the broker manager to HTTPS. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.24..</param>
        /// <param name="webManagerRedirectHttpOverrideTlsPort">The HTTPS port that HTTP requests will be redirected towards in a HTTP 301 redirect response. Zero is a special value that means use the value specified for the SEMP TLS port value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;. Available since 2.24..</param>
        public Broker(AuthClientCertRevocationCheckModeEnum? authClientCertRevocationCheckMode = default(AuthClientCertRevocationCheckModeEnum?), long? configSyncAuthenticationClientCertMaxChainDepth = default(long?), bool? configSyncAuthenticationClientCertValidateDateEnabled = default(bool?), long? configSyncClientProfileTcpInitialCongestionWindow = default(long?), long? configSyncClientProfileTcpKeepaliveCount = default(long?), long? configSyncClientProfileTcpKeepaliveIdle = default(long?), long? configSyncClientProfileTcpKeepaliveInterval = default(long?), long? configSyncClientProfileTcpMaxWindow = default(long?), long? configSyncClientProfileTcpMss = default(long?), bool? configSyncEnabled = default(bool?), bool? configSyncSynchronizeUsernameEnabled = default(bool?), bool? configSyncTlsEnabled = default(bool?), string guaranteedMsgingDefragmentationScheduleDayList = default(string), bool? guaranteedMsgingDefragmentationScheduleEnabled = default(bool?), string guaranteedMsgingDefragmentationScheduleTimeList = default(string), bool? guaranteedMsgingDefragmentationThresholdEnabled = default(bool?), long? guaranteedMsgingDefragmentationThresholdFragmentationPercentage = default(long?), long? guaranteedMsgingDefragmentationThresholdMinInterval = default(long?), long? guaranteedMsgingDefragmentationThresholdUsagePercentage = default(long?), bool? guaranteedMsgingEnabled = default(bool?), EventThreshold guaranteedMsgingEventCacheUsageThreshold = default(EventThreshold), EventThresholdByPercent guaranteedMsgingEventDeliveredUnackedThreshold = default(EventThresholdByPercent), EventThresholdByPercent guaranteedMsgingEventDiskUsageThreshold = default(EventThresholdByPercent), EventThreshold guaranteedMsgingEventEgressFlowCountThreshold = default(EventThreshold), EventThreshold guaranteedMsgingEventEndpointCountThreshold = default(EventThreshold), EventThreshold guaranteedMsgingEventIngressFlowCountThreshold = default(EventThreshold), EventThresholdByPercent guaranteedMsgingEventMsgCountThreshold = default(EventThresholdByPercent), EventThresholdByPercent guaranteedMsgingEventMsgSpoolFileCountThreshold = default(EventThresholdByPercent), EventThreshold guaranteedMsgingEventMsgSpoolUsageThreshold = default(EventThreshold), EventThreshold guaranteedMsgingEventTransactedSessionCountThreshold = default(EventThreshold), EventThresholdByPercent guaranteedMsgingEventTransactedSessionResourceCountThreshold = default(EventThresholdByPercent), EventThreshold guaranteedMsgingEventTransactionCountThreshold = default(EventThreshold), int? guaranteedMsgingMaxCacheUsage = default(int?), long? guaranteedMsgingMaxMsgSpoolUsage = default(long?), long? guaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout = default(long?), long? guaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout = default(long?), GuaranteedMsgingTransactionReplicationCompatibilityModeEnum? guaranteedMsgingTransactionReplicationCompatibilityMode = default(GuaranteedMsgingTransactionReplicationCompatibilityModeEnum?), string oauthProfileDefault = default(string), bool? serviceAmqpEnabled = default(bool?), long? serviceAmqpTlsListenPort = default(long?), EventThreshold serviceEventConnectionCountThreshold = default(EventThreshold), bool? serviceHealthCheckEnabled = default(bool?), long? serviceHealthCheckListenPort = default(long?), bool? serviceHealthCheckTlsEnabled = default(bool?), long? serviceHealthCheckTlsListenPort = default(long?), bool? serviceMateLinkEnabled = default(bool?), long? serviceMateLinkListenPort = default(long?), bool? serviceMqttEnabled = default(bool?), bool? serviceMsgBackboneEnabled = default(bool?), bool? serviceRedundancyEnabled = default(bool?), long? serviceRedundancyFirstListenPort = default(long?), EventThreshold serviceRestEventOutgoingConnectionCountThreshold = default(EventThreshold), bool? serviceRestIncomingEnabled = default(bool?), bool? serviceRestOutgoingEnabled = default(bool?), bool? serviceSempCorsAllowAnyHostEnabled = default(bool?), bool? serviceSempLegacyTimeoutEnabled = default(bool?), bool? serviceSempPlainTextEnabled = default(bool?), long? serviceSempPlainTextListenPort = default(long?), int? serviceSempSessionIdleTimeout = default(int?), int? serviceSempSessionMaxLifetime = default(int?), bool? serviceSempTlsEnabled = default(bool?), long? serviceSempTlsListenPort = default(long?), long? serviceSmfCompressionListenPort = default(long?), bool? serviceSmfEnabled = default(bool?), EventThreshold serviceSmfEventConnectionCountThreshold = default(EventThreshold), long? serviceSmfPlainTextListenPort = default(long?), long? serviceSmfRoutingControlListenPort = default(long?), long? serviceSmfTlsListenPort = default(long?), EventThreshold serviceTlsEventConnectionCountThreshold = default(EventThreshold), bool? serviceWebTransportEnabled = default(bool?), long? serviceWebTransportPlainTextListenPort = default(long?), long? serviceWebTransportTlsListenPort = default(long?), string serviceWebTransportWebUrlSuffix = default(string), bool? tlsBlockVersion11Enabled = default(bool?), string tlsCipherSuiteManagementList = default(string), string tlsCipherSuiteMsgBackboneList = default(string), string tlsCipherSuiteSecureShellList = default(string), bool? tlsCrimeExploitProtectionEnabled = default(bool?), string tlsServerCertContent = default(string), string tlsServerCertPassword = default(string), bool? tlsStandardDomainCertificateAuthoritiesEnabled = default(bool?), int? tlsTicketLifetime = default(int?), bool? webManagerAllowUnencryptedWizardsEnabled = default(bool?), string webManagerCustomization = default(string), bool? webManagerRedirectHttpEnabled = default(bool?), int? webManagerRedirectHttpOverrideTlsPort = default(int?))
        {
            this.AuthClientCertRevocationCheckMode = authClientCertRevocationCheckMode;
            this.ConfigSyncAuthenticationClientCertMaxChainDepth = configSyncAuthenticationClientCertMaxChainDepth;
            this.ConfigSyncAuthenticationClientCertValidateDateEnabled = configSyncAuthenticationClientCertValidateDateEnabled;
            this.ConfigSyncClientProfileTcpInitialCongestionWindow = configSyncClientProfileTcpInitialCongestionWindow;
            this.ConfigSyncClientProfileTcpKeepaliveCount = configSyncClientProfileTcpKeepaliveCount;
            this.ConfigSyncClientProfileTcpKeepaliveIdle = configSyncClientProfileTcpKeepaliveIdle;
            this.ConfigSyncClientProfileTcpKeepaliveInterval = configSyncClientProfileTcpKeepaliveInterval;
            this.ConfigSyncClientProfileTcpMaxWindow = configSyncClientProfileTcpMaxWindow;
            this.ConfigSyncClientProfileTcpMss = configSyncClientProfileTcpMss;
            this.ConfigSyncEnabled = configSyncEnabled;
            this.ConfigSyncSynchronizeUsernameEnabled = configSyncSynchronizeUsernameEnabled;
            this.ConfigSyncTlsEnabled = configSyncTlsEnabled;
            this.GuaranteedMsgingDefragmentationScheduleDayList = guaranteedMsgingDefragmentationScheduleDayList;
            this.GuaranteedMsgingDefragmentationScheduleEnabled = guaranteedMsgingDefragmentationScheduleEnabled;
            this.GuaranteedMsgingDefragmentationScheduleTimeList = guaranteedMsgingDefragmentationScheduleTimeList;
            this.GuaranteedMsgingDefragmentationThresholdEnabled = guaranteedMsgingDefragmentationThresholdEnabled;
            this.GuaranteedMsgingDefragmentationThresholdFragmentationPercentage = guaranteedMsgingDefragmentationThresholdFragmentationPercentage;
            this.GuaranteedMsgingDefragmentationThresholdMinInterval = guaranteedMsgingDefragmentationThresholdMinInterval;
            this.GuaranteedMsgingDefragmentationThresholdUsagePercentage = guaranteedMsgingDefragmentationThresholdUsagePercentage;
            this.GuaranteedMsgingEnabled = guaranteedMsgingEnabled;
            this.GuaranteedMsgingEventCacheUsageThreshold = guaranteedMsgingEventCacheUsageThreshold;
            this.GuaranteedMsgingEventDeliveredUnackedThreshold = guaranteedMsgingEventDeliveredUnackedThreshold;
            this.GuaranteedMsgingEventDiskUsageThreshold = guaranteedMsgingEventDiskUsageThreshold;
            this.GuaranteedMsgingEventEgressFlowCountThreshold = guaranteedMsgingEventEgressFlowCountThreshold;
            this.GuaranteedMsgingEventEndpointCountThreshold = guaranteedMsgingEventEndpointCountThreshold;
            this.GuaranteedMsgingEventIngressFlowCountThreshold = guaranteedMsgingEventIngressFlowCountThreshold;
            this.GuaranteedMsgingEventMsgCountThreshold = guaranteedMsgingEventMsgCountThreshold;
            this.GuaranteedMsgingEventMsgSpoolFileCountThreshold = guaranteedMsgingEventMsgSpoolFileCountThreshold;
            this.GuaranteedMsgingEventMsgSpoolUsageThreshold = guaranteedMsgingEventMsgSpoolUsageThreshold;
            this.GuaranteedMsgingEventTransactedSessionCountThreshold = guaranteedMsgingEventTransactedSessionCountThreshold;
            this.GuaranteedMsgingEventTransactedSessionResourceCountThreshold = guaranteedMsgingEventTransactedSessionResourceCountThreshold;
            this.GuaranteedMsgingEventTransactionCountThreshold = guaranteedMsgingEventTransactionCountThreshold;
            this.GuaranteedMsgingMaxCacheUsage = guaranteedMsgingMaxCacheUsage;
            this.GuaranteedMsgingMaxMsgSpoolUsage = guaranteedMsgingMaxMsgSpoolUsage;
            this.GuaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout = guaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout;
            this.GuaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout = guaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout;
            this.GuaranteedMsgingTransactionReplicationCompatibilityMode = guaranteedMsgingTransactionReplicationCompatibilityMode;
            this.OauthProfileDefault = oauthProfileDefault;
            this.ServiceAmqpEnabled = serviceAmqpEnabled;
            this.ServiceAmqpTlsListenPort = serviceAmqpTlsListenPort;
            this.ServiceEventConnectionCountThreshold = serviceEventConnectionCountThreshold;
            this.ServiceHealthCheckEnabled = serviceHealthCheckEnabled;
            this.ServiceHealthCheckListenPort = serviceHealthCheckListenPort;
            this.ServiceHealthCheckTlsEnabled = serviceHealthCheckTlsEnabled;
            this.ServiceHealthCheckTlsListenPort = serviceHealthCheckTlsListenPort;
            this.ServiceMateLinkEnabled = serviceMateLinkEnabled;
            this.ServiceMateLinkListenPort = serviceMateLinkListenPort;
            this.ServiceMqttEnabled = serviceMqttEnabled;
            this.ServiceMsgBackboneEnabled = serviceMsgBackboneEnabled;
            this.ServiceRedundancyEnabled = serviceRedundancyEnabled;
            this.ServiceRedundancyFirstListenPort = serviceRedundancyFirstListenPort;
            this.ServiceRestEventOutgoingConnectionCountThreshold = serviceRestEventOutgoingConnectionCountThreshold;
            this.ServiceRestIncomingEnabled = serviceRestIncomingEnabled;
            this.ServiceRestOutgoingEnabled = serviceRestOutgoingEnabled;
            this.ServiceSempCorsAllowAnyHostEnabled = serviceSempCorsAllowAnyHostEnabled;
            this.ServiceSempLegacyTimeoutEnabled = serviceSempLegacyTimeoutEnabled;
            this.ServiceSempPlainTextEnabled = serviceSempPlainTextEnabled;
            this.ServiceSempPlainTextListenPort = serviceSempPlainTextListenPort;
            this.ServiceSempSessionIdleTimeout = serviceSempSessionIdleTimeout;
            this.ServiceSempSessionMaxLifetime = serviceSempSessionMaxLifetime;
            this.ServiceSempTlsEnabled = serviceSempTlsEnabled;
            this.ServiceSempTlsListenPort = serviceSempTlsListenPort;
            this.ServiceSmfCompressionListenPort = serviceSmfCompressionListenPort;
            this.ServiceSmfEnabled = serviceSmfEnabled;
            this.ServiceSmfEventConnectionCountThreshold = serviceSmfEventConnectionCountThreshold;
            this.ServiceSmfPlainTextListenPort = serviceSmfPlainTextListenPort;
            this.ServiceSmfRoutingControlListenPort = serviceSmfRoutingControlListenPort;
            this.ServiceSmfTlsListenPort = serviceSmfTlsListenPort;
            this.ServiceTlsEventConnectionCountThreshold = serviceTlsEventConnectionCountThreshold;
            this.ServiceWebTransportEnabled = serviceWebTransportEnabled;
            this.ServiceWebTransportPlainTextListenPort = serviceWebTransportPlainTextListenPort;
            this.ServiceWebTransportTlsListenPort = serviceWebTransportTlsListenPort;
            this.ServiceWebTransportWebUrlSuffix = serviceWebTransportWebUrlSuffix;
            this.TlsBlockVersion11Enabled = tlsBlockVersion11Enabled;
            this.TlsCipherSuiteManagementList = tlsCipherSuiteManagementList;
            this.TlsCipherSuiteMsgBackboneList = tlsCipherSuiteMsgBackboneList;
            this.TlsCipherSuiteSecureShellList = tlsCipherSuiteSecureShellList;
            this.TlsCrimeExploitProtectionEnabled = tlsCrimeExploitProtectionEnabled;
            this.TlsServerCertContent = tlsServerCertContent;
            this.TlsServerCertPassword = tlsServerCertPassword;
            this.TlsStandardDomainCertificateAuthoritiesEnabled = tlsStandardDomainCertificateAuthoritiesEnabled;
            this.TlsTicketLifetime = tlsTicketLifetime;
            this.WebManagerAllowUnencryptedWizardsEnabled = webManagerAllowUnencryptedWizardsEnabled;
            this.WebManagerCustomization = webManagerCustomization;
            this.WebManagerRedirectHttpEnabled = webManagerRedirectHttpEnabled;
            this.WebManagerRedirectHttpOverrideTlsPort = webManagerRedirectHttpOverrideTlsPort;
        }
        

        /// <summary>
        /// The maximum depth for a client certificate chain. The depth of a chain is defined as the number of signing CA certificates that are present in the chain back to a trusted self-signed root CA certificate. The default value is &#x60;3&#x60;. Available since 2.22.
        /// </summary>
        /// <value>The maximum depth for a client certificate chain. The depth of a chain is defined as the number of signing CA certificates that are present in the chain back to a trusted self-signed root CA certificate. The default value is &#x60;3&#x60;. Available since 2.22.</value>
        [DataMember(Name="configSyncAuthenticationClientCertMaxChainDepth", EmitDefaultValue=false)]
        public long? ConfigSyncAuthenticationClientCertMaxChainDepth { get; set; }

        /// <summary>
        /// Enable or disable validation of the \&quot;Not Before\&quot; and \&quot;Not After\&quot; validity dates in the authentication certificate(s). The default value is &#x60;true&#x60;. Available since 2.22.
        /// </summary>
        /// <value>Enable or disable validation of the \&quot;Not Before\&quot; and \&quot;Not After\&quot; validity dates in the authentication certificate(s). The default value is &#x60;true&#x60;. Available since 2.22.</value>
        [DataMember(Name="configSyncAuthenticationClientCertValidateDateEnabled", EmitDefaultValue=false)]
        public bool? ConfigSyncAuthenticationClientCertValidateDateEnabled { get; set; }

        /// <summary>
        /// The TCP initial congestion window size for Config Sync clients, in multiples of the TCP Maximum Segment Size (MSS). Changing the value from its default of 2 results in non-compliance with RFC 2581. Contact support before changing this value. The default value is &#x60;2&#x60;. Available since 2.22.
        /// </summary>
        /// <value>The TCP initial congestion window size for Config Sync clients, in multiples of the TCP Maximum Segment Size (MSS). Changing the value from its default of 2 results in non-compliance with RFC 2581. Contact support before changing this value. The default value is &#x60;2&#x60;. Available since 2.22.</value>
        [DataMember(Name="configSyncClientProfileTcpInitialCongestionWindow", EmitDefaultValue=false)]
        public long? ConfigSyncClientProfileTcpInitialCongestionWindow { get; set; }

        /// <summary>
        /// The number of TCP keepalive retransmissions to a client using the Client Profile before declaring that it is not available. The default value is &#x60;5&#x60;. Available since 2.22.
        /// </summary>
        /// <value>The number of TCP keepalive retransmissions to a client using the Client Profile before declaring that it is not available. The default value is &#x60;5&#x60;. Available since 2.22.</value>
        [DataMember(Name="configSyncClientProfileTcpKeepaliveCount", EmitDefaultValue=false)]
        public long? ConfigSyncClientProfileTcpKeepaliveCount { get; set; }

        /// <summary>
        /// The amount of time a client connection using the Client Profile must remain idle before TCP begins sending keepalive probes, in seconds. The default value is &#x60;3&#x60;. Available since 2.22.
        /// </summary>
        /// <value>The amount of time a client connection using the Client Profile must remain idle before TCP begins sending keepalive probes, in seconds. The default value is &#x60;3&#x60;. Available since 2.22.</value>
        [DataMember(Name="configSyncClientProfileTcpKeepaliveIdle", EmitDefaultValue=false)]
        public long? ConfigSyncClientProfileTcpKeepaliveIdle { get; set; }

        /// <summary>
        /// The amount of time between TCP keepalive retransmissions to a client using the Client Profile when no acknowledgement is received, in seconds. The default value is &#x60;1&#x60;. Available since 2.22.
        /// </summary>
        /// <value>The amount of time between TCP keepalive retransmissions to a client using the Client Profile when no acknowledgement is received, in seconds. The default value is &#x60;1&#x60;. Available since 2.22.</value>
        [DataMember(Name="configSyncClientProfileTcpKeepaliveInterval", EmitDefaultValue=false)]
        public long? ConfigSyncClientProfileTcpKeepaliveInterval { get; set; }

        /// <summary>
        /// The TCP maximum window size for clients using the Client Profile, in kilobytes. Changes are applied to all existing connections. This setting is ignored on the software broker. The default value is &#x60;256&#x60;. Available since 2.22.
        /// </summary>
        /// <value>The TCP maximum window size for clients using the Client Profile, in kilobytes. Changes are applied to all existing connections. This setting is ignored on the software broker. The default value is &#x60;256&#x60;. Available since 2.22.</value>
        [DataMember(Name="configSyncClientProfileTcpMaxWindow", EmitDefaultValue=false)]
        public long? ConfigSyncClientProfileTcpMaxWindow { get; set; }

        /// <summary>
        /// The TCP maximum segment size for clients using the Client Profile, in bytes. Changes are applied to all existing connections. The default value is &#x60;1460&#x60;. Available since 2.22.
        /// </summary>
        /// <value>The TCP maximum segment size for clients using the Client Profile, in bytes. Changes are applied to all existing connections. The default value is &#x60;1460&#x60;. Available since 2.22.</value>
        [DataMember(Name="configSyncClientProfileTcpMss", EmitDefaultValue=false)]
        public long? ConfigSyncClientProfileTcpMss { get; set; }

        /// <summary>
        /// Enable or disable configuration synchronization for High Availability or Disaster Recovery. The default value is &#x60;false&#x60;. Available since 2.22.
        /// </summary>
        /// <value>Enable or disable configuration synchronization for High Availability or Disaster Recovery. The default value is &#x60;false&#x60;. Available since 2.22.</value>
        [DataMember(Name="configSyncEnabled", EmitDefaultValue=false)]
        public bool? ConfigSyncEnabled { get; set; }

        /// <summary>
        /// Enable or disable the synchronizing of usernames within High Availability groups. The transition from not synchronizing to synchronizing will cause the High Availability mate to fall out of sync. Recommendation: leave this as enabled. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.22.
        /// </summary>
        /// <value>Enable or disable the synchronizing of usernames within High Availability groups. The transition from not synchronizing to synchronizing will cause the High Availability mate to fall out of sync. Recommendation: leave this as enabled. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.22.</value>
        [DataMember(Name="configSyncSynchronizeUsernameEnabled", EmitDefaultValue=false)]
        public bool? ConfigSyncSynchronizeUsernameEnabled { get; set; }

        /// <summary>
        /// Enable or disable the use of TLS encryption of the configuration synchronization communications between brokers in High Availability groups and/or Disaster Recovery sites. The default value is &#x60;false&#x60;. Available since 2.22.
        /// </summary>
        /// <value>Enable or disable the use of TLS encryption of the configuration synchronization communications between brokers in High Availability groups and/or Disaster Recovery sites. The default value is &#x60;false&#x60;. Available since 2.22.</value>
        [DataMember(Name="configSyncTlsEnabled", EmitDefaultValue=false)]
        public bool? ConfigSyncTlsEnabled { get; set; }

        /// <summary>
        /// The days of the week to schedule defragmentation runs, specified as \&quot;daily\&quot; or as a comma-separated list of days. Days must be specified as \&quot;Sun\&quot;, \&quot;Mon\&quot;, \&quot;Tue\&quot;, \&quot;Wed\&quot;, \&quot;Thu\&quot;, \&quot;Fri, or \&quot;Sat\&quot;, with no spaces, and in sorted order from Sunday to Saturday. Please note \&quot;Sun,Mon,Tue,Wed,Thu,Fri,Sat\&quot; is not allowed, use \&quot;daily\&quot; instead. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;daily\&quot;&#x60;. Available since 2.25.
        /// </summary>
        /// <value>The days of the week to schedule defragmentation runs, specified as \&quot;daily\&quot; or as a comma-separated list of days. Days must be specified as \&quot;Sun\&quot;, \&quot;Mon\&quot;, \&quot;Tue\&quot;, \&quot;Wed\&quot;, \&quot;Thu\&quot;, \&quot;Fri, or \&quot;Sat\&quot;, with no spaces, and in sorted order from Sunday to Saturday. Please note \&quot;Sun,Mon,Tue,Wed,Thu,Fri,Sat\&quot; is not allowed, use \&quot;daily\&quot; instead. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;daily\&quot;&#x60;. Available since 2.25.</value>
        [DataMember(Name="guaranteedMsgingDefragmentationScheduleDayList", EmitDefaultValue=false)]
        public string GuaranteedMsgingDefragmentationScheduleDayList { get; set; }

        /// <summary>
        /// Enable or disable schedule-based defragmentation of Guaranteed Messaging spool files. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.25.
        /// </summary>
        /// <value>Enable or disable schedule-based defragmentation of Guaranteed Messaging spool files. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.25.</value>
        [DataMember(Name="guaranteedMsgingDefragmentationScheduleEnabled", EmitDefaultValue=false)]
        public bool? GuaranteedMsgingDefragmentationScheduleEnabled { get; set; }

        /// <summary>
        /// The times of the day to schedule defragmentation runs, specified as \&quot;hourly\&quot; or as a comma-separated list of 24-hour times in the form hh:mm, or h:mm. There must be no spaces, and times (up to 4) must be in sorted order from 0:00 to 23:59. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;0:00\&quot;&#x60;. Available since 2.25.
        /// </summary>
        /// <value>The times of the day to schedule defragmentation runs, specified as \&quot;hourly\&quot; or as a comma-separated list of 24-hour times in the form hh:mm, or h:mm. There must be no spaces, and times (up to 4) must be in sorted order from 0:00 to 23:59. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;0:00\&quot;&#x60;. Available since 2.25.</value>
        [DataMember(Name="guaranteedMsgingDefragmentationScheduleTimeList", EmitDefaultValue=false)]
        public string GuaranteedMsgingDefragmentationScheduleTimeList { get; set; }

        /// <summary>
        /// Enable or disable threshold-based defragmentation of Guaranteed Messaging spool files. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.25.
        /// </summary>
        /// <value>Enable or disable threshold-based defragmentation of Guaranteed Messaging spool files. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.25.</value>
        [DataMember(Name="guaranteedMsgingDefragmentationThresholdEnabled", EmitDefaultValue=false)]
        public bool? GuaranteedMsgingDefragmentationThresholdEnabled { get; set; }

        /// <summary>
        /// Percentage of spool fragmentation needed to trigger defragmentation run. The minimum value allowed is 30%. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;50&#x60;. Available since 2.25.
        /// </summary>
        /// <value>Percentage of spool fragmentation needed to trigger defragmentation run. The minimum value allowed is 30%. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;50&#x60;. Available since 2.25.</value>
        [DataMember(Name="guaranteedMsgingDefragmentationThresholdFragmentationPercentage", EmitDefaultValue=false)]
        public long? GuaranteedMsgingDefragmentationThresholdFragmentationPercentage { get; set; }

        /// <summary>
        /// Minimum interval of time (in minutes) between defragmentation runs triggered by thresholds. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;15&#x60;. Available since 2.25.
        /// </summary>
        /// <value>Minimum interval of time (in minutes) between defragmentation runs triggered by thresholds. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;15&#x60;. Available since 2.25.</value>
        [DataMember(Name="guaranteedMsgingDefragmentationThresholdMinInterval", EmitDefaultValue=false)]
        public long? GuaranteedMsgingDefragmentationThresholdMinInterval { get; set; }

        /// <summary>
        /// Percentage of spool usage needed to trigger defragmentation run. The minimum value allowed is 30%. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;50&#x60;. Available since 2.25.
        /// </summary>
        /// <value>Percentage of spool usage needed to trigger defragmentation run. The minimum value allowed is 30%. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;50&#x60;. Available since 2.25.</value>
        [DataMember(Name="guaranteedMsgingDefragmentationThresholdUsagePercentage", EmitDefaultValue=false)]
        public long? GuaranteedMsgingDefragmentationThresholdUsagePercentage { get; set; }

        /// <summary>
        /// Enable or disable Guaranteed Messaging. The default value is &#x60;false&#x60;. Available since 2.18.
        /// </summary>
        /// <value>Enable or disable Guaranteed Messaging. The default value is &#x60;false&#x60;. Available since 2.18.</value>
        [DataMember(Name="guaranteedMsgingEnabled", EmitDefaultValue=false)]
        public bool? GuaranteedMsgingEnabled { get; set; }

        /// <summary>
        /// Gets or Sets GuaranteedMsgingEventCacheUsageThreshold
        /// </summary>
        [DataMember(Name="guaranteedMsgingEventCacheUsageThreshold", EmitDefaultValue=false)]
        public EventThreshold GuaranteedMsgingEventCacheUsageThreshold { get; set; }

        /// <summary>
        /// Gets or Sets GuaranteedMsgingEventDeliveredUnackedThreshold
        /// </summary>
        [DataMember(Name="guaranteedMsgingEventDeliveredUnackedThreshold", EmitDefaultValue=false)]
        public EventThresholdByPercent GuaranteedMsgingEventDeliveredUnackedThreshold { get; set; }

        /// <summary>
        /// Gets or Sets GuaranteedMsgingEventDiskUsageThreshold
        /// </summary>
        [DataMember(Name="guaranteedMsgingEventDiskUsageThreshold", EmitDefaultValue=false)]
        public EventThresholdByPercent GuaranteedMsgingEventDiskUsageThreshold { get; set; }

        /// <summary>
        /// Gets or Sets GuaranteedMsgingEventEgressFlowCountThreshold
        /// </summary>
        [DataMember(Name="guaranteedMsgingEventEgressFlowCountThreshold", EmitDefaultValue=false)]
        public EventThreshold GuaranteedMsgingEventEgressFlowCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets GuaranteedMsgingEventEndpointCountThreshold
        /// </summary>
        [DataMember(Name="guaranteedMsgingEventEndpointCountThreshold", EmitDefaultValue=false)]
        public EventThreshold GuaranteedMsgingEventEndpointCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets GuaranteedMsgingEventIngressFlowCountThreshold
        /// </summary>
        [DataMember(Name="guaranteedMsgingEventIngressFlowCountThreshold", EmitDefaultValue=false)]
        public EventThreshold GuaranteedMsgingEventIngressFlowCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets GuaranteedMsgingEventMsgCountThreshold
        /// </summary>
        [DataMember(Name="guaranteedMsgingEventMsgCountThreshold", EmitDefaultValue=false)]
        public EventThresholdByPercent GuaranteedMsgingEventMsgCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets GuaranteedMsgingEventMsgSpoolFileCountThreshold
        /// </summary>
        [DataMember(Name="guaranteedMsgingEventMsgSpoolFileCountThreshold", EmitDefaultValue=false)]
        public EventThresholdByPercent GuaranteedMsgingEventMsgSpoolFileCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets GuaranteedMsgingEventMsgSpoolUsageThreshold
        /// </summary>
        [DataMember(Name="guaranteedMsgingEventMsgSpoolUsageThreshold", EmitDefaultValue=false)]
        public EventThreshold GuaranteedMsgingEventMsgSpoolUsageThreshold { get; set; }

        /// <summary>
        /// Gets or Sets GuaranteedMsgingEventTransactedSessionCountThreshold
        /// </summary>
        [DataMember(Name="guaranteedMsgingEventTransactedSessionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold GuaranteedMsgingEventTransactedSessionCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets GuaranteedMsgingEventTransactedSessionResourceCountThreshold
        /// </summary>
        [DataMember(Name="guaranteedMsgingEventTransactedSessionResourceCountThreshold", EmitDefaultValue=false)]
        public EventThresholdByPercent GuaranteedMsgingEventTransactedSessionResourceCountThreshold { get; set; }

        /// <summary>
        /// Gets or Sets GuaranteedMsgingEventTransactionCountThreshold
        /// </summary>
        [DataMember(Name="guaranteedMsgingEventTransactionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold GuaranteedMsgingEventTransactionCountThreshold { get; set; }

        /// <summary>
        /// Guaranteed messaging cache usage limit. Expressed as a maximum percentage of the NAB&#x27;s egress queueing. resources that the guaranteed message cache is allowed to use. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;10&#x60;. Available since 2.18.
        /// </summary>
        /// <value>Guaranteed messaging cache usage limit. Expressed as a maximum percentage of the NAB&#x27;s egress queueing. resources that the guaranteed message cache is allowed to use. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;10&#x60;. Available since 2.18.</value>
        [DataMember(Name="guaranteedMsgingMaxCacheUsage", EmitDefaultValue=false)]
        public int? GuaranteedMsgingMaxCacheUsage { get; set; }

        /// <summary>
        /// The maximum total message spool usage allowed across all VPNs on this broker, in megabytes. Recommendation: the maximum value should be less than 90% of the disk space allocated for the guaranteed message spool. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1500&#x60;. Available since 2.18.
        /// </summary>
        /// <value>The maximum total message spool usage allowed across all VPNs on this broker, in megabytes. Recommendation: the maximum value should be less than 90% of the disk space allocated for the guaranteed message spool. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1500&#x60;. Available since 2.18.</value>
        [DataMember(Name="guaranteedMsgingMaxMsgSpoolUsage", EmitDefaultValue=false)]
        public long? GuaranteedMsgingMaxMsgSpoolUsage { get; set; }

        /// <summary>
        /// The maximum time, in milliseconds, that can be tolerated for remote acknowledgement of synchronization messages before which the remote system will be considered out of sync. The default value is &#x60;10000&#x60;. Available since 2.18.
        /// </summary>
        /// <value>The maximum time, in milliseconds, that can be tolerated for remote acknowledgement of synchronization messages before which the remote system will be considered out of sync. The default value is &#x60;10000&#x60;. Available since 2.18.</value>
        [DataMember(Name="guaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout", EmitDefaultValue=false)]
        public long? GuaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout { get; set; }

        /// <summary>
        /// The maximum time, in milliseconds, that can be tolerated for remote disk writes before which the remote system will be considered out of sync. The default value is &#x60;10000&#x60;. Available since 2.18.
        /// </summary>
        /// <value>The maximum time, in milliseconds, that can be tolerated for remote disk writes before which the remote system will be considered out of sync. The default value is &#x60;10000&#x60;. Available since 2.18.</value>
        [DataMember(Name="guaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout", EmitDefaultValue=false)]
        public long? GuaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout { get; set; }


        /// <summary>
        /// The default OAuth profile for OAuth authenticated SEMP requests. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.24.
        /// </summary>
        /// <value>The default OAuth profile for OAuth authenticated SEMP requests. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.24.</value>
        [DataMember(Name="oauthProfileDefault", EmitDefaultValue=false)]
        public string OauthProfileDefault { get; set; }

        /// <summary>
        /// Enable or disable the AMQP service. When disabled new AMQP Clients may not connect through the global or per-VPN AMQP listen-ports, and all currently connected AMQP Clients are immediately disconnected. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Enable or disable the AMQP service. When disabled new AMQP Clients may not connect through the global or per-VPN AMQP listen-ports, and all currently connected AMQP Clients are immediately disconnected. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceAmqpEnabled", EmitDefaultValue=false)]
        public bool? ServiceAmqpEnabled { get; set; }

        /// <summary>
        /// TCP port number that AMQP clients can use to connect to the broker using raw TCP over TLS. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceAmqpEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;. Available since 2.17.
        /// </summary>
        /// <value>TCP port number that AMQP clients can use to connect to the broker using raw TCP over TLS. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceAmqpEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceAmqpTlsListenPort", EmitDefaultValue=false)]
        public long? ServiceAmqpTlsListenPort { get; set; }

        /// <summary>
        /// Gets or Sets ServiceEventConnectionCountThreshold
        /// </summary>
        [DataMember(Name="serviceEventConnectionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold ServiceEventConnectionCountThreshold { get; set; }

        /// <summary>
        /// Enable or disable the plain-text health-check service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Enable or disable the plain-text health-check service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceHealthCheckEnabled", EmitDefaultValue=false)]
        public bool? ServiceHealthCheckEnabled { get; set; }

        /// <summary>
        /// The port number for the plain-text health-check service. The port must be unique across the message backbone. The health-check service must be disabled to change the port. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceHealthCheckEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;5550&#x60;. Available since 2.17.
        /// </summary>
        /// <value>The port number for the plain-text health-check service. The port must be unique across the message backbone. The health-check service must be disabled to change the port. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceHealthCheckEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;5550&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceHealthCheckListenPort", EmitDefaultValue=false)]
        public long? ServiceHealthCheckListenPort { get; set; }

        /// <summary>
        /// Enable or disable the TLS health-check service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.34.
        /// </summary>
        /// <value>Enable or disable the TLS health-check service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.34.</value>
        [DataMember(Name="serviceHealthCheckTlsEnabled", EmitDefaultValue=false)]
        public bool? ServiceHealthCheckTlsEnabled { get; set; }

        /// <summary>
        /// The port number for the TLS health-check service. The port must be unique across the message backbone. The health-check service must be disabled to change the port. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceHealthCheckTlsEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;. Available since 2.34.
        /// </summary>
        /// <value>The port number for the TLS health-check service. The port must be unique across the message backbone. The health-check service must be disabled to change the port. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceHealthCheckTlsEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;. Available since 2.34.</value>
        [DataMember(Name="serviceHealthCheckTlsListenPort", EmitDefaultValue=false)]
        public long? ServiceHealthCheckTlsListenPort { get; set; }

        /// <summary>
        /// Enable or disable the mate-link service. The default value is &#x60;true&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Enable or disable the mate-link service. The default value is &#x60;true&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceMateLinkEnabled", EmitDefaultValue=false)]
        public bool? ServiceMateLinkEnabled { get; set; }

        /// <summary>
        /// The port number for the mate-link service. The port must be unique across the message backbone. The mate-link service must be disabled to change the port. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMateLinkEnabled will be temporarily set to false to apply the change. The default value is &#x60;8741&#x60;. Available since 2.17.
        /// </summary>
        /// <value>The port number for the mate-link service. The port must be unique across the message backbone. The mate-link service must be disabled to change the port. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceMateLinkEnabled will be temporarily set to false to apply the change. The default value is &#x60;8741&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceMateLinkListenPort", EmitDefaultValue=false)]
        public long? ServiceMateLinkListenPort { get; set; }

        /// <summary>
        /// Enable or disable the MQTT service. When disabled new MQTT Clients may not connect through the per-VPN MQTT listen-ports, and all currently connected MQTT Clients are immediately disconnected. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Enable or disable the MQTT service. When disabled new MQTT Clients may not connect through the per-VPN MQTT listen-ports, and all currently connected MQTT Clients are immediately disconnected. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceMqttEnabled", EmitDefaultValue=false)]
        public bool? ServiceMqttEnabled { get; set; }

        /// <summary>
        /// Enable or disable the msg-backbone service. When disabled new Clients may not connect through global or per-VPN listen-ports, and all currently connected Clients are immediately disconnected. The default value is &#x60;true&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Enable or disable the msg-backbone service. When disabled new Clients may not connect through global or per-VPN listen-ports, and all currently connected Clients are immediately disconnected. The default value is &#x60;true&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceMsgBackboneEnabled", EmitDefaultValue=false)]
        public bool? ServiceMsgBackboneEnabled { get; set; }

        /// <summary>
        /// Enable or disable the redundancy service. The default value is &#x60;true&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Enable or disable the redundancy service. The default value is &#x60;true&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceRedundancyEnabled", EmitDefaultValue=false)]
        public bool? ServiceRedundancyEnabled { get; set; }

        /// <summary>
        /// The first listen-port used for the redundancy service. Redundancy uses this port and the subsequent 2 ports. These port must be unique across the message backbone. The redundancy service must be disabled to change this port. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceRedundancyEnabled will be temporarily set to false to apply the change. The default value is &#x60;8300&#x60;. Available since 2.17.
        /// </summary>
        /// <value>The first listen-port used for the redundancy service. Redundancy uses this port and the subsequent 2 ports. These port must be unique across the message backbone. The redundancy service must be disabled to change this port. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceRedundancyEnabled will be temporarily set to false to apply the change. The default value is &#x60;8300&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceRedundancyFirstListenPort", EmitDefaultValue=false)]
        public long? ServiceRedundancyFirstListenPort { get; set; }

        /// <summary>
        /// Gets or Sets ServiceRestEventOutgoingConnectionCountThreshold
        /// </summary>
        [DataMember(Name="serviceRestEventOutgoingConnectionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold ServiceRestEventOutgoingConnectionCountThreshold { get; set; }

        /// <summary>
        /// Enable or disable the REST service incoming connections on the router. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Enable or disable the REST service incoming connections on the router. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceRestIncomingEnabled", EmitDefaultValue=false)]
        public bool? ServiceRestIncomingEnabled { get; set; }

        /// <summary>
        /// Enable or disable the REST service outgoing connections on the router. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Enable or disable the REST service outgoing connections on the router. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceRestOutgoingEnabled", EmitDefaultValue=false)]
        public bool? ServiceRestOutgoingEnabled { get; set; }

        /// <summary>
        /// Enable or disable cross origin resource requests for the SEMP service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.24.
        /// </summary>
        /// <value>Enable or disable cross origin resource requests for the SEMP service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.24.</value>
        [DataMember(Name="serviceSempCorsAllowAnyHostEnabled", EmitDefaultValue=false)]
        public bool? ServiceSempCorsAllowAnyHostEnabled { get; set; }

        /// <summary>
        /// Enable or disable extended SEMP timeouts for paged GETs. When a request times out, it returns the current page of content, even if the page is not full.  When enabled, the timeout is 60 seconds. When disabled, the timeout is 5 seconds.  The recommended setting is disabled (no legacy-timeout).  This parameter is intended as a temporary workaround to be used until SEMP clients can handle short pages.  This setting will be removed in a future release. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.18.
        /// </summary>
        /// <value>Enable or disable extended SEMP timeouts for paged GETs. When a request times out, it returns the current page of content, even if the page is not full.  When enabled, the timeout is 60 seconds. When disabled, the timeout is 5 seconds.  The recommended setting is disabled (no legacy-timeout).  This parameter is intended as a temporary workaround to be used until SEMP clients can handle short pages.  This setting will be removed in a future release. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.18.</value>
        [DataMember(Name="serviceSempLegacyTimeoutEnabled", EmitDefaultValue=false)]
        public bool? ServiceSempLegacyTimeoutEnabled { get; set; }

        /// <summary>
        /// Enable or disable plain-text SEMP service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Enable or disable plain-text SEMP service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceSempPlainTextEnabled", EmitDefaultValue=false)]
        public bool? ServiceSempPlainTextEnabled { get; set; }

        /// <summary>
        /// The TCP port for plain-text SEMP client connections. This attribute cannot be cannot be changed while serviceSempPlainTextEnabled are set to true. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;80&#x60;. Available since 2.17.
        /// </summary>
        /// <value>The TCP port for plain-text SEMP client connections. This attribute cannot be cannot be changed while serviceSempPlainTextEnabled are set to true. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;80&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceSempPlainTextListenPort", EmitDefaultValue=false)]
        public long? ServiceSempPlainTextListenPort { get; set; }

        /// <summary>
        /// The session idle timeout, in minutes. Sessions will be invalidated if there is no activity in this period of time. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;15&#x60;. Available since 2.21.
        /// </summary>
        /// <value>The session idle timeout, in minutes. Sessions will be invalidated if there is no activity in this period of time. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;15&#x60;. Available since 2.21.</value>
        [DataMember(Name="serviceSempSessionIdleTimeout", EmitDefaultValue=false)]
        public int? ServiceSempSessionIdleTimeout { get; set; }

        /// <summary>
        /// The maximum lifetime of a session, in minutes. Sessions will be invalidated after this period of time, regardless of activity. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;43200&#x60;. Available since 2.21.
        /// </summary>
        /// <value>The maximum lifetime of a session, in minutes. Sessions will be invalidated after this period of time, regardless of activity. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;43200&#x60;. Available since 2.21.</value>
        [DataMember(Name="serviceSempSessionMaxLifetime", EmitDefaultValue=false)]
        public int? ServiceSempSessionMaxLifetime { get; set; }

        /// <summary>
        /// Enable or disable TLS SEMP service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Enable or disable TLS SEMP service. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceSempTlsEnabled", EmitDefaultValue=false)]
        public bool? ServiceSempTlsEnabled { get; set; }

        /// <summary>
        /// The TCP port for TLS SEMP client connections. This attribute cannot be cannot be changed while serviceSempTlsEnabled are set to true. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1943&#x60;. Available since 2.17.
        /// </summary>
        /// <value>The TCP port for TLS SEMP client connections. This attribute cannot be cannot be changed while serviceSempTlsEnabled are set to true. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1943&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceSempTlsListenPort", EmitDefaultValue=false)]
        public long? ServiceSempTlsListenPort { get; set; }

        /// <summary>
        /// TCP port number that SMF clients can use to connect to the broker using raw compression TCP. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceSmfEnabled will be temporarily set to false to apply the change. The default value is &#x60;55003&#x60;. Available since 2.17.
        /// </summary>
        /// <value>TCP port number that SMF clients can use to connect to the broker using raw compression TCP. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceSmfEnabled will be temporarily set to false to apply the change. The default value is &#x60;55003&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceSmfCompressionListenPort", EmitDefaultValue=false)]
        public long? ServiceSmfCompressionListenPort { get; set; }

        /// <summary>
        /// Enable or disable the SMF service. When disabled new SMF Clients may not connect through the global listen-ports, and all currently connected SMF Clients are immediately disconnected. The default value is &#x60;true&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Enable or disable the SMF service. When disabled new SMF Clients may not connect through the global listen-ports, and all currently connected SMF Clients are immediately disconnected. The default value is &#x60;true&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceSmfEnabled", EmitDefaultValue=false)]
        public bool? ServiceSmfEnabled { get; set; }

        /// <summary>
        /// Gets or Sets ServiceSmfEventConnectionCountThreshold
        /// </summary>
        [DataMember(Name="serviceSmfEventConnectionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold ServiceSmfEventConnectionCountThreshold { get; set; }

        /// <summary>
        /// TCP port number that SMF clients can use to connect to the broker using raw TCP. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceSmfEnabled will be temporarily set to false to apply the change. The default value is &#x60;55555&#x60;. Available since 2.17.
        /// </summary>
        /// <value>TCP port number that SMF clients can use to connect to the broker using raw TCP. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceSmfEnabled will be temporarily set to false to apply the change. The default value is &#x60;55555&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceSmfPlainTextListenPort", EmitDefaultValue=false)]
        public long? ServiceSmfPlainTextListenPort { get; set; }

        /// <summary>
        /// TCP port number that SMF clients can use to connect to the broker using raw routing control TCP. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceSmfEnabled will be temporarily set to false to apply the change. The default value is &#x60;55556&#x60;. Available since 2.17.
        /// </summary>
        /// <value>TCP port number that SMF clients can use to connect to the broker using raw routing control TCP. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceSmfEnabled will be temporarily set to false to apply the change. The default value is &#x60;55556&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceSmfRoutingControlListenPort", EmitDefaultValue=false)]
        public long? ServiceSmfRoutingControlListenPort { get; set; }

        /// <summary>
        /// TCP port number that SMF clients can use to connect to the broker using raw TCP over TLS. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceSmfEnabled will be temporarily set to false to apply the change. The default value is &#x60;55443&#x60;. Available since 2.17.
        /// </summary>
        /// <value>TCP port number that SMF clients can use to connect to the broker using raw TCP over TLS. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceSmfEnabled will be temporarily set to false to apply the change. The default value is &#x60;55443&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceSmfTlsListenPort", EmitDefaultValue=false)]
        public long? ServiceSmfTlsListenPort { get; set; }

        /// <summary>
        /// Gets or Sets ServiceTlsEventConnectionCountThreshold
        /// </summary>
        [DataMember(Name="serviceTlsEventConnectionCountThreshold", EmitDefaultValue=false)]
        public EventThreshold ServiceTlsEventConnectionCountThreshold { get; set; }

        /// <summary>
        /// Enable or disable the web-transport service. When disabled new web-transport Clients may not connect through the global listen-ports, and all currently connected web-transport Clients are immediately disconnected. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Enable or disable the web-transport service. When disabled new web-transport Clients may not connect through the global listen-ports, and all currently connected web-transport Clients are immediately disconnected. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceWebTransportEnabled", EmitDefaultValue=false)]
        public bool? ServiceWebTransportEnabled { get; set; }

        /// <summary>
        /// The TCP port for plain-text WEB client connections. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceWebTransportEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;8008&#x60;. Available since 2.17.
        /// </summary>
        /// <value>The TCP port for plain-text WEB client connections. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceWebTransportEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;8008&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceWebTransportPlainTextListenPort", EmitDefaultValue=false)]
        public long? ServiceWebTransportPlainTextListenPort { get; set; }

        /// <summary>
        /// The TCP port for TLS WEB client connections. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceWebTransportEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1443&#x60;. Available since 2.17.
        /// </summary>
        /// <value>The TCP port for TLS WEB client connections. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceWebTransportEnabled will be temporarily set to false to apply the change. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;1443&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceWebTransportTlsListenPort", EmitDefaultValue=false)]
        public long? ServiceWebTransportTlsListenPort { get; set; }

        /// <summary>
        /// Used to specify the Web URL suffix that will be used by Web clients when communicating with the broker. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceWebTransportEnabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.17.
        /// </summary>
        /// <value>Used to specify the Web URL suffix that will be used by Web clients when communicating with the broker. Modifying this attribute while the object (or the relevant part of the object) is administratively enabled may be service impacting as serviceWebTransportEnabled will be temporarily set to false to apply the change. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.17.</value>
        [DataMember(Name="serviceWebTransportWebUrlSuffix", EmitDefaultValue=false)]
        public string ServiceWebTransportWebUrlSuffix { get; set; }

        /// <summary>
        /// Enable or disable the blocking of TLS version 1.1 connections. When blocked, all existing incoming and outgoing TLS 1.1 connections with Clients, SEMP users, and LDAP servers remain connected while new connections are blocked. Note that support for TLS 1.1 will eventually be discontinued, at which time TLS 1.1 connections will be blocked regardless of this setting. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.
        /// </summary>
        /// <value>Enable or disable the blocking of TLS version 1.1 connections. When blocked, all existing incoming and outgoing TLS 1.1 connections with Clients, SEMP users, and LDAP servers remain connected while new connections are blocked. Note that support for TLS 1.1 will eventually be discontinued, at which time TLS 1.1 connections will be blocked regardless of this setting. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;.</value>
        [DataMember(Name="tlsBlockVersion11Enabled", EmitDefaultValue=false)]
        public bool? TlsBlockVersion11Enabled { get; set; }

        /// <summary>
        /// The colon-separated list of cipher suites used for TLS management connections (e.g. SEMP, LDAP). The value \&quot;default\&quot; implies all supported suites ordered from most secure to least secure. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;.
        /// </summary>
        /// <value>The colon-separated list of cipher suites used for TLS management connections (e.g. SEMP, LDAP). The value \&quot;default\&quot; implies all supported suites ordered from most secure to least secure. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;.</value>
        [DataMember(Name="tlsCipherSuiteManagementList", EmitDefaultValue=false)]
        public string TlsCipherSuiteManagementList { get; set; }

        /// <summary>
        /// The colon-separated list of cipher suites used for TLS data connections (e.g. client pub/sub). The value \&quot;default\&quot; implies all supported suites ordered from most secure to least secure. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;.
        /// </summary>
        /// <value>The colon-separated list of cipher suites used for TLS data connections (e.g. client pub/sub). The value \&quot;default\&quot; implies all supported suites ordered from most secure to least secure. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;.</value>
        [DataMember(Name="tlsCipherSuiteMsgBackboneList", EmitDefaultValue=false)]
        public string TlsCipherSuiteMsgBackboneList { get; set; }

        /// <summary>
        /// The colon-separated list of cipher suites used for TLS secure shell connections (e.g. SSH, SFTP, SCP). The value \&quot;default\&quot; implies all supported suites ordered from most secure to least secure. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;.
        /// </summary>
        /// <value>The colon-separated list of cipher suites used for TLS secure shell connections (e.g. SSH, SFTP, SCP). The value \&quot;default\&quot; implies all supported suites ordered from most secure to least secure. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;default\&quot;&#x60;.</value>
        [DataMember(Name="tlsCipherSuiteSecureShellList", EmitDefaultValue=false)]
        public string TlsCipherSuiteSecureShellList { get; set; }

        /// <summary>
        /// Enable or disable protection against the CRIME exploit. When enabled, TLS+compressed messaging performance is degraded. This protection should only be disabled if sufficient ACL and authentication features are being employed such that a potential attacker does not have sufficient access to trigger the exploit. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.
        /// </summary>
        /// <value>Enable or disable protection against the CRIME exploit. When enabled, TLS+compressed messaging performance is degraded. This protection should only be disabled if sufficient ACL and authentication features are being employed such that a potential attacker does not have sufficient access to trigger the exploit. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;.</value>
        [DataMember(Name="tlsCrimeExploitProtectionEnabled", EmitDefaultValue=false)]
        public bool? TlsCrimeExploitProtectionEnabled { get; set; }

        /// <summary>
        /// The PEM formatted content for the server certificate used for TLS connections. It must consist of a private key and between one and three certificates comprising the certificate trust chain. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The PEM formatted content for the server certificate used for TLS connections. It must consist of a private key and between one and three certificates comprising the certificate trust chain. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="tlsServerCertContent", EmitDefaultValue=false)]
        public string TlsServerCertContent { get; set; }

        /// <summary>
        /// The password for the server certificate used for TLS connections. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. The default value is &#x60;\&quot;\&quot;&#x60;.
        /// </summary>
        /// <value>The password for the server certificate used for TLS connections. This attribute is absent from a GET and not updated when absent in a PUT, subject to the exceptions in note 4. Changing this attribute requires an HTTPS connection. The default value is &#x60;\&quot;\&quot;&#x60;.</value>
        [DataMember(Name="tlsServerCertPassword", EmitDefaultValue=false)]
        public string TlsServerCertPassword { get; set; }

        /// <summary>
        /// Enable or disable the standard domain certificate authority list. The default value is &#x60;true&#x60;. Available since 2.19.
        /// </summary>
        /// <value>Enable or disable the standard domain certificate authority list. The default value is &#x60;true&#x60;. Available since 2.19.</value>
        [DataMember(Name="tlsStandardDomainCertificateAuthoritiesEnabled", EmitDefaultValue=false)]
        public bool? TlsStandardDomainCertificateAuthoritiesEnabled { get; set; }

        /// <summary>
        /// The TLS ticket lifetime in seconds. When a client connects with TLS, a session with a session ticket is created using the TLS ticket lifetime which determines how long the client has to resume the session. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;86400&#x60;.
        /// </summary>
        /// <value>The TLS ticket lifetime in seconds. When a client connects with TLS, a session with a session ticket is created using the TLS ticket lifetime which determines how long the client has to resume the session. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;86400&#x60;.</value>
        [DataMember(Name="tlsTicketLifetime", EmitDefaultValue=false)]
        public int? TlsTicketLifetime { get; set; }

        /// <summary>
        /// Enable or disable the use of unencrypted wizards in the Web-based Manager UI. This setting should be left at its default on all production systems or other systems that need to be secure.  Enabling this option will permit the broker to forward plain-text data to other brokers, making important information or credentials available for snooping. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.28.
        /// </summary>
        /// <value>Enable or disable the use of unencrypted wizards in the Web-based Manager UI. This setting should be left at its default on all production systems or other systems that need to be secure.  Enabling this option will permit the broker to forward plain-text data to other brokers, making important information or credentials available for snooping. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;false&#x60;. Available since 2.28.</value>
        [DataMember(Name="webManagerAllowUnencryptedWizardsEnabled", EmitDefaultValue=false)]
        public bool? WebManagerAllowUnencryptedWizardsEnabled { get; set; }

        /// <summary>
        /// Reserved for internal use by Solace. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.25.
        /// </summary>
        /// <value>Reserved for internal use by Solace. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;\&quot;\&quot;&#x60;. Available since 2.25.</value>
        [DataMember(Name="webManagerCustomization", EmitDefaultValue=false)]
        public string WebManagerCustomization { get; set; }

        /// <summary>
        /// Enable or disable redirection of HTTP requests for the broker manager to HTTPS. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.24.
        /// </summary>
        /// <value>Enable or disable redirection of HTTP requests for the broker manager to HTTPS. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;true&#x60;. Available since 2.24.</value>
        [DataMember(Name="webManagerRedirectHttpEnabled", EmitDefaultValue=false)]
        public bool? WebManagerRedirectHttpEnabled { get; set; }

        /// <summary>
        /// The HTTPS port that HTTP requests will be redirected towards in a HTTP 301 redirect response. Zero is a special value that means use the value specified for the SEMP TLS port value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;. Available since 2.24.
        /// </summary>
        /// <value>The HTTPS port that HTTP requests will be redirected towards in a HTTP 301 redirect response. Zero is a special value that means use the value specified for the SEMP TLS port value. Changes to this attribute are synchronized to HA mates via config-sync. The default value is &#x60;0&#x60;. Available since 2.24.</value>
        [DataMember(Name="webManagerRedirectHttpOverrideTlsPort", EmitDefaultValue=false)]
        public int? WebManagerRedirectHttpOverrideTlsPort { get; set; }

        /// <summary>
        /// Returns the string presentation of the object
        /// </summary>
        /// <returns>String presentation of the object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("class Broker {\n");
            sb.Append("  AuthClientCertRevocationCheckMode: ").Append(AuthClientCertRevocationCheckMode).Append("\n");
            sb.Append("  ConfigSyncAuthenticationClientCertMaxChainDepth: ").Append(ConfigSyncAuthenticationClientCertMaxChainDepth).Append("\n");
            sb.Append("  ConfigSyncAuthenticationClientCertValidateDateEnabled: ").Append(ConfigSyncAuthenticationClientCertValidateDateEnabled).Append("\n");
            sb.Append("  ConfigSyncClientProfileTcpInitialCongestionWindow: ").Append(ConfigSyncClientProfileTcpInitialCongestionWindow).Append("\n");
            sb.Append("  ConfigSyncClientProfileTcpKeepaliveCount: ").Append(ConfigSyncClientProfileTcpKeepaliveCount).Append("\n");
            sb.Append("  ConfigSyncClientProfileTcpKeepaliveIdle: ").Append(ConfigSyncClientProfileTcpKeepaliveIdle).Append("\n");
            sb.Append("  ConfigSyncClientProfileTcpKeepaliveInterval: ").Append(ConfigSyncClientProfileTcpKeepaliveInterval).Append("\n");
            sb.Append("  ConfigSyncClientProfileTcpMaxWindow: ").Append(ConfigSyncClientProfileTcpMaxWindow).Append("\n");
            sb.Append("  ConfigSyncClientProfileTcpMss: ").Append(ConfigSyncClientProfileTcpMss).Append("\n");
            sb.Append("  ConfigSyncEnabled: ").Append(ConfigSyncEnabled).Append("\n");
            sb.Append("  ConfigSyncSynchronizeUsernameEnabled: ").Append(ConfigSyncSynchronizeUsernameEnabled).Append("\n");
            sb.Append("  ConfigSyncTlsEnabled: ").Append(ConfigSyncTlsEnabled).Append("\n");
            sb.Append("  GuaranteedMsgingDefragmentationScheduleDayList: ").Append(GuaranteedMsgingDefragmentationScheduleDayList).Append("\n");
            sb.Append("  GuaranteedMsgingDefragmentationScheduleEnabled: ").Append(GuaranteedMsgingDefragmentationScheduleEnabled).Append("\n");
            sb.Append("  GuaranteedMsgingDefragmentationScheduleTimeList: ").Append(GuaranteedMsgingDefragmentationScheduleTimeList).Append("\n");
            sb.Append("  GuaranteedMsgingDefragmentationThresholdEnabled: ").Append(GuaranteedMsgingDefragmentationThresholdEnabled).Append("\n");
            sb.Append("  GuaranteedMsgingDefragmentationThresholdFragmentationPercentage: ").Append(GuaranteedMsgingDefragmentationThresholdFragmentationPercentage).Append("\n");
            sb.Append("  GuaranteedMsgingDefragmentationThresholdMinInterval: ").Append(GuaranteedMsgingDefragmentationThresholdMinInterval).Append("\n");
            sb.Append("  GuaranteedMsgingDefragmentationThresholdUsagePercentage: ").Append(GuaranteedMsgingDefragmentationThresholdUsagePercentage).Append("\n");
            sb.Append("  GuaranteedMsgingEnabled: ").Append(GuaranteedMsgingEnabled).Append("\n");
            sb.Append("  GuaranteedMsgingEventCacheUsageThreshold: ").Append(GuaranteedMsgingEventCacheUsageThreshold).Append("\n");
            sb.Append("  GuaranteedMsgingEventDeliveredUnackedThreshold: ").Append(GuaranteedMsgingEventDeliveredUnackedThreshold).Append("\n");
            sb.Append("  GuaranteedMsgingEventDiskUsageThreshold: ").Append(GuaranteedMsgingEventDiskUsageThreshold).Append("\n");
            sb.Append("  GuaranteedMsgingEventEgressFlowCountThreshold: ").Append(GuaranteedMsgingEventEgressFlowCountThreshold).Append("\n");
            sb.Append("  GuaranteedMsgingEventEndpointCountThreshold: ").Append(GuaranteedMsgingEventEndpointCountThreshold).Append("\n");
            sb.Append("  GuaranteedMsgingEventIngressFlowCountThreshold: ").Append(GuaranteedMsgingEventIngressFlowCountThreshold).Append("\n");
            sb.Append("  GuaranteedMsgingEventMsgCountThreshold: ").Append(GuaranteedMsgingEventMsgCountThreshold).Append("\n");
            sb.Append("  GuaranteedMsgingEventMsgSpoolFileCountThreshold: ").Append(GuaranteedMsgingEventMsgSpoolFileCountThreshold).Append("\n");
            sb.Append("  GuaranteedMsgingEventMsgSpoolUsageThreshold: ").Append(GuaranteedMsgingEventMsgSpoolUsageThreshold).Append("\n");
            sb.Append("  GuaranteedMsgingEventTransactedSessionCountThreshold: ").Append(GuaranteedMsgingEventTransactedSessionCountThreshold).Append("\n");
            sb.Append("  GuaranteedMsgingEventTransactedSessionResourceCountThreshold: ").Append(GuaranteedMsgingEventTransactedSessionResourceCountThreshold).Append("\n");
            sb.Append("  GuaranteedMsgingEventTransactionCountThreshold: ").Append(GuaranteedMsgingEventTransactionCountThreshold).Append("\n");
            sb.Append("  GuaranteedMsgingMaxCacheUsage: ").Append(GuaranteedMsgingMaxCacheUsage).Append("\n");
            sb.Append("  GuaranteedMsgingMaxMsgSpoolUsage: ").Append(GuaranteedMsgingMaxMsgSpoolUsage).Append("\n");
            sb.Append("  GuaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout: ").Append(GuaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout).Append("\n");
            sb.Append("  GuaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout: ").Append(GuaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout).Append("\n");
            sb.Append("  GuaranteedMsgingTransactionReplicationCompatibilityMode: ").Append(GuaranteedMsgingTransactionReplicationCompatibilityMode).Append("\n");
            sb.Append("  OauthProfileDefault: ").Append(OauthProfileDefault).Append("\n");
            sb.Append("  ServiceAmqpEnabled: ").Append(ServiceAmqpEnabled).Append("\n");
            sb.Append("  ServiceAmqpTlsListenPort: ").Append(ServiceAmqpTlsListenPort).Append("\n");
            sb.Append("  ServiceEventConnectionCountThreshold: ").Append(ServiceEventConnectionCountThreshold).Append("\n");
            sb.Append("  ServiceHealthCheckEnabled: ").Append(ServiceHealthCheckEnabled).Append("\n");
            sb.Append("  ServiceHealthCheckListenPort: ").Append(ServiceHealthCheckListenPort).Append("\n");
            sb.Append("  ServiceHealthCheckTlsEnabled: ").Append(ServiceHealthCheckTlsEnabled).Append("\n");
            sb.Append("  ServiceHealthCheckTlsListenPort: ").Append(ServiceHealthCheckTlsListenPort).Append("\n");
            sb.Append("  ServiceMateLinkEnabled: ").Append(ServiceMateLinkEnabled).Append("\n");
            sb.Append("  ServiceMateLinkListenPort: ").Append(ServiceMateLinkListenPort).Append("\n");
            sb.Append("  ServiceMqttEnabled: ").Append(ServiceMqttEnabled).Append("\n");
            sb.Append("  ServiceMsgBackboneEnabled: ").Append(ServiceMsgBackboneEnabled).Append("\n");
            sb.Append("  ServiceRedundancyEnabled: ").Append(ServiceRedundancyEnabled).Append("\n");
            sb.Append("  ServiceRedundancyFirstListenPort: ").Append(ServiceRedundancyFirstListenPort).Append("\n");
            sb.Append("  ServiceRestEventOutgoingConnectionCountThreshold: ").Append(ServiceRestEventOutgoingConnectionCountThreshold).Append("\n");
            sb.Append("  ServiceRestIncomingEnabled: ").Append(ServiceRestIncomingEnabled).Append("\n");
            sb.Append("  ServiceRestOutgoingEnabled: ").Append(ServiceRestOutgoingEnabled).Append("\n");
            sb.Append("  ServiceSempCorsAllowAnyHostEnabled: ").Append(ServiceSempCorsAllowAnyHostEnabled).Append("\n");
            sb.Append("  ServiceSempLegacyTimeoutEnabled: ").Append(ServiceSempLegacyTimeoutEnabled).Append("\n");
            sb.Append("  ServiceSempPlainTextEnabled: ").Append(ServiceSempPlainTextEnabled).Append("\n");
            sb.Append("  ServiceSempPlainTextListenPort: ").Append(ServiceSempPlainTextListenPort).Append("\n");
            sb.Append("  ServiceSempSessionIdleTimeout: ").Append(ServiceSempSessionIdleTimeout).Append("\n");
            sb.Append("  ServiceSempSessionMaxLifetime: ").Append(ServiceSempSessionMaxLifetime).Append("\n");
            sb.Append("  ServiceSempTlsEnabled: ").Append(ServiceSempTlsEnabled).Append("\n");
            sb.Append("  ServiceSempTlsListenPort: ").Append(ServiceSempTlsListenPort).Append("\n");
            sb.Append("  ServiceSmfCompressionListenPort: ").Append(ServiceSmfCompressionListenPort).Append("\n");
            sb.Append("  ServiceSmfEnabled: ").Append(ServiceSmfEnabled).Append("\n");
            sb.Append("  ServiceSmfEventConnectionCountThreshold: ").Append(ServiceSmfEventConnectionCountThreshold).Append("\n");
            sb.Append("  ServiceSmfPlainTextListenPort: ").Append(ServiceSmfPlainTextListenPort).Append("\n");
            sb.Append("  ServiceSmfRoutingControlListenPort: ").Append(ServiceSmfRoutingControlListenPort).Append("\n");
            sb.Append("  ServiceSmfTlsListenPort: ").Append(ServiceSmfTlsListenPort).Append("\n");
            sb.Append("  ServiceTlsEventConnectionCountThreshold: ").Append(ServiceTlsEventConnectionCountThreshold).Append("\n");
            sb.Append("  ServiceWebTransportEnabled: ").Append(ServiceWebTransportEnabled).Append("\n");
            sb.Append("  ServiceWebTransportPlainTextListenPort: ").Append(ServiceWebTransportPlainTextListenPort).Append("\n");
            sb.Append("  ServiceWebTransportTlsListenPort: ").Append(ServiceWebTransportTlsListenPort).Append("\n");
            sb.Append("  ServiceWebTransportWebUrlSuffix: ").Append(ServiceWebTransportWebUrlSuffix).Append("\n");
            sb.Append("  TlsBlockVersion11Enabled: ").Append(TlsBlockVersion11Enabled).Append("\n");
            sb.Append("  TlsCipherSuiteManagementList: ").Append(TlsCipherSuiteManagementList).Append("\n");
            sb.Append("  TlsCipherSuiteMsgBackboneList: ").Append(TlsCipherSuiteMsgBackboneList).Append("\n");
            sb.Append("  TlsCipherSuiteSecureShellList: ").Append(TlsCipherSuiteSecureShellList).Append("\n");
            sb.Append("  TlsCrimeExploitProtectionEnabled: ").Append(TlsCrimeExploitProtectionEnabled).Append("\n");
            sb.Append("  TlsServerCertContent: ").Append(TlsServerCertContent).Append("\n");
            sb.Append("  TlsServerCertPassword: ").Append(TlsServerCertPassword).Append("\n");
            sb.Append("  TlsStandardDomainCertificateAuthoritiesEnabled: ").Append(TlsStandardDomainCertificateAuthoritiesEnabled).Append("\n");
            sb.Append("  TlsTicketLifetime: ").Append(TlsTicketLifetime).Append("\n");
            sb.Append("  WebManagerAllowUnencryptedWizardsEnabled: ").Append(WebManagerAllowUnencryptedWizardsEnabled).Append("\n");
            sb.Append("  WebManagerCustomization: ").Append(WebManagerCustomization).Append("\n");
            sb.Append("  WebManagerRedirectHttpEnabled: ").Append(WebManagerRedirectHttpEnabled).Append("\n");
            sb.Append("  WebManagerRedirectHttpOverrideTlsPort: ").Append(WebManagerRedirectHttpOverrideTlsPort).Append("\n");
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
            return this.Equals(input as Broker);
        }

        /// <summary>
        /// Returns true if Broker instances are equal
        /// </summary>
        /// <param name="input">Instance of Broker to be compared</param>
        /// <returns>Boolean</returns>
        public bool Equals(Broker input)
        {
            if (input == null)
                return false;

            return 
                (
                    this.AuthClientCertRevocationCheckMode == input.AuthClientCertRevocationCheckMode ||
                    (this.AuthClientCertRevocationCheckMode != null &&
                    this.AuthClientCertRevocationCheckMode.Equals(input.AuthClientCertRevocationCheckMode))
                ) && 
                (
                    this.ConfigSyncAuthenticationClientCertMaxChainDepth == input.ConfigSyncAuthenticationClientCertMaxChainDepth ||
                    (this.ConfigSyncAuthenticationClientCertMaxChainDepth != null &&
                    this.ConfigSyncAuthenticationClientCertMaxChainDepth.Equals(input.ConfigSyncAuthenticationClientCertMaxChainDepth))
                ) && 
                (
                    this.ConfigSyncAuthenticationClientCertValidateDateEnabled == input.ConfigSyncAuthenticationClientCertValidateDateEnabled ||
                    (this.ConfigSyncAuthenticationClientCertValidateDateEnabled != null &&
                    this.ConfigSyncAuthenticationClientCertValidateDateEnabled.Equals(input.ConfigSyncAuthenticationClientCertValidateDateEnabled))
                ) && 
                (
                    this.ConfigSyncClientProfileTcpInitialCongestionWindow == input.ConfigSyncClientProfileTcpInitialCongestionWindow ||
                    (this.ConfigSyncClientProfileTcpInitialCongestionWindow != null &&
                    this.ConfigSyncClientProfileTcpInitialCongestionWindow.Equals(input.ConfigSyncClientProfileTcpInitialCongestionWindow))
                ) && 
                (
                    this.ConfigSyncClientProfileTcpKeepaliveCount == input.ConfigSyncClientProfileTcpKeepaliveCount ||
                    (this.ConfigSyncClientProfileTcpKeepaliveCount != null &&
                    this.ConfigSyncClientProfileTcpKeepaliveCount.Equals(input.ConfigSyncClientProfileTcpKeepaliveCount))
                ) && 
                (
                    this.ConfigSyncClientProfileTcpKeepaliveIdle == input.ConfigSyncClientProfileTcpKeepaliveIdle ||
                    (this.ConfigSyncClientProfileTcpKeepaliveIdle != null &&
                    this.ConfigSyncClientProfileTcpKeepaliveIdle.Equals(input.ConfigSyncClientProfileTcpKeepaliveIdle))
                ) && 
                (
                    this.ConfigSyncClientProfileTcpKeepaliveInterval == input.ConfigSyncClientProfileTcpKeepaliveInterval ||
                    (this.ConfigSyncClientProfileTcpKeepaliveInterval != null &&
                    this.ConfigSyncClientProfileTcpKeepaliveInterval.Equals(input.ConfigSyncClientProfileTcpKeepaliveInterval))
                ) && 
                (
                    this.ConfigSyncClientProfileTcpMaxWindow == input.ConfigSyncClientProfileTcpMaxWindow ||
                    (this.ConfigSyncClientProfileTcpMaxWindow != null &&
                    this.ConfigSyncClientProfileTcpMaxWindow.Equals(input.ConfigSyncClientProfileTcpMaxWindow))
                ) && 
                (
                    this.ConfigSyncClientProfileTcpMss == input.ConfigSyncClientProfileTcpMss ||
                    (this.ConfigSyncClientProfileTcpMss != null &&
                    this.ConfigSyncClientProfileTcpMss.Equals(input.ConfigSyncClientProfileTcpMss))
                ) && 
                (
                    this.ConfigSyncEnabled == input.ConfigSyncEnabled ||
                    (this.ConfigSyncEnabled != null &&
                    this.ConfigSyncEnabled.Equals(input.ConfigSyncEnabled))
                ) && 
                (
                    this.ConfigSyncSynchronizeUsernameEnabled == input.ConfigSyncSynchronizeUsernameEnabled ||
                    (this.ConfigSyncSynchronizeUsernameEnabled != null &&
                    this.ConfigSyncSynchronizeUsernameEnabled.Equals(input.ConfigSyncSynchronizeUsernameEnabled))
                ) && 
                (
                    this.ConfigSyncTlsEnabled == input.ConfigSyncTlsEnabled ||
                    (this.ConfigSyncTlsEnabled != null &&
                    this.ConfigSyncTlsEnabled.Equals(input.ConfigSyncTlsEnabled))
                ) && 
                (
                    this.GuaranteedMsgingDefragmentationScheduleDayList == input.GuaranteedMsgingDefragmentationScheduleDayList ||
                    (this.GuaranteedMsgingDefragmentationScheduleDayList != null &&
                    this.GuaranteedMsgingDefragmentationScheduleDayList.Equals(input.GuaranteedMsgingDefragmentationScheduleDayList))
                ) && 
                (
                    this.GuaranteedMsgingDefragmentationScheduleEnabled == input.GuaranteedMsgingDefragmentationScheduleEnabled ||
                    (this.GuaranteedMsgingDefragmentationScheduleEnabled != null &&
                    this.GuaranteedMsgingDefragmentationScheduleEnabled.Equals(input.GuaranteedMsgingDefragmentationScheduleEnabled))
                ) && 
                (
                    this.GuaranteedMsgingDefragmentationScheduleTimeList == input.GuaranteedMsgingDefragmentationScheduleTimeList ||
                    (this.GuaranteedMsgingDefragmentationScheduleTimeList != null &&
                    this.GuaranteedMsgingDefragmentationScheduleTimeList.Equals(input.GuaranteedMsgingDefragmentationScheduleTimeList))
                ) && 
                (
                    this.GuaranteedMsgingDefragmentationThresholdEnabled == input.GuaranteedMsgingDefragmentationThresholdEnabled ||
                    (this.GuaranteedMsgingDefragmentationThresholdEnabled != null &&
                    this.GuaranteedMsgingDefragmentationThresholdEnabled.Equals(input.GuaranteedMsgingDefragmentationThresholdEnabled))
                ) && 
                (
                    this.GuaranteedMsgingDefragmentationThresholdFragmentationPercentage == input.GuaranteedMsgingDefragmentationThresholdFragmentationPercentage ||
                    (this.GuaranteedMsgingDefragmentationThresholdFragmentationPercentage != null &&
                    this.GuaranteedMsgingDefragmentationThresholdFragmentationPercentage.Equals(input.GuaranteedMsgingDefragmentationThresholdFragmentationPercentage))
                ) && 
                (
                    this.GuaranteedMsgingDefragmentationThresholdMinInterval == input.GuaranteedMsgingDefragmentationThresholdMinInterval ||
                    (this.GuaranteedMsgingDefragmentationThresholdMinInterval != null &&
                    this.GuaranteedMsgingDefragmentationThresholdMinInterval.Equals(input.GuaranteedMsgingDefragmentationThresholdMinInterval))
                ) && 
                (
                    this.GuaranteedMsgingDefragmentationThresholdUsagePercentage == input.GuaranteedMsgingDefragmentationThresholdUsagePercentage ||
                    (this.GuaranteedMsgingDefragmentationThresholdUsagePercentage != null &&
                    this.GuaranteedMsgingDefragmentationThresholdUsagePercentage.Equals(input.GuaranteedMsgingDefragmentationThresholdUsagePercentage))
                ) && 
                (
                    this.GuaranteedMsgingEnabled == input.GuaranteedMsgingEnabled ||
                    (this.GuaranteedMsgingEnabled != null &&
                    this.GuaranteedMsgingEnabled.Equals(input.GuaranteedMsgingEnabled))
                ) && 
                (
                    this.GuaranteedMsgingEventCacheUsageThreshold == input.GuaranteedMsgingEventCacheUsageThreshold ||
                    (this.GuaranteedMsgingEventCacheUsageThreshold != null &&
                    this.GuaranteedMsgingEventCacheUsageThreshold.Equals(input.GuaranteedMsgingEventCacheUsageThreshold))
                ) && 
                (
                    this.GuaranteedMsgingEventDeliveredUnackedThreshold == input.GuaranteedMsgingEventDeliveredUnackedThreshold ||
                    (this.GuaranteedMsgingEventDeliveredUnackedThreshold != null &&
                    this.GuaranteedMsgingEventDeliveredUnackedThreshold.Equals(input.GuaranteedMsgingEventDeliveredUnackedThreshold))
                ) && 
                (
                    this.GuaranteedMsgingEventDiskUsageThreshold == input.GuaranteedMsgingEventDiskUsageThreshold ||
                    (this.GuaranteedMsgingEventDiskUsageThreshold != null &&
                    this.GuaranteedMsgingEventDiskUsageThreshold.Equals(input.GuaranteedMsgingEventDiskUsageThreshold))
                ) && 
                (
                    this.GuaranteedMsgingEventEgressFlowCountThreshold == input.GuaranteedMsgingEventEgressFlowCountThreshold ||
                    (this.GuaranteedMsgingEventEgressFlowCountThreshold != null &&
                    this.GuaranteedMsgingEventEgressFlowCountThreshold.Equals(input.GuaranteedMsgingEventEgressFlowCountThreshold))
                ) && 
                (
                    this.GuaranteedMsgingEventEndpointCountThreshold == input.GuaranteedMsgingEventEndpointCountThreshold ||
                    (this.GuaranteedMsgingEventEndpointCountThreshold != null &&
                    this.GuaranteedMsgingEventEndpointCountThreshold.Equals(input.GuaranteedMsgingEventEndpointCountThreshold))
                ) && 
                (
                    this.GuaranteedMsgingEventIngressFlowCountThreshold == input.GuaranteedMsgingEventIngressFlowCountThreshold ||
                    (this.GuaranteedMsgingEventIngressFlowCountThreshold != null &&
                    this.GuaranteedMsgingEventIngressFlowCountThreshold.Equals(input.GuaranteedMsgingEventIngressFlowCountThreshold))
                ) && 
                (
                    this.GuaranteedMsgingEventMsgCountThreshold == input.GuaranteedMsgingEventMsgCountThreshold ||
                    (this.GuaranteedMsgingEventMsgCountThreshold != null &&
                    this.GuaranteedMsgingEventMsgCountThreshold.Equals(input.GuaranteedMsgingEventMsgCountThreshold))
                ) && 
                (
                    this.GuaranteedMsgingEventMsgSpoolFileCountThreshold == input.GuaranteedMsgingEventMsgSpoolFileCountThreshold ||
                    (this.GuaranteedMsgingEventMsgSpoolFileCountThreshold != null &&
                    this.GuaranteedMsgingEventMsgSpoolFileCountThreshold.Equals(input.GuaranteedMsgingEventMsgSpoolFileCountThreshold))
                ) && 
                (
                    this.GuaranteedMsgingEventMsgSpoolUsageThreshold == input.GuaranteedMsgingEventMsgSpoolUsageThreshold ||
                    (this.GuaranteedMsgingEventMsgSpoolUsageThreshold != null &&
                    this.GuaranteedMsgingEventMsgSpoolUsageThreshold.Equals(input.GuaranteedMsgingEventMsgSpoolUsageThreshold))
                ) && 
                (
                    this.GuaranteedMsgingEventTransactedSessionCountThreshold == input.GuaranteedMsgingEventTransactedSessionCountThreshold ||
                    (this.GuaranteedMsgingEventTransactedSessionCountThreshold != null &&
                    this.GuaranteedMsgingEventTransactedSessionCountThreshold.Equals(input.GuaranteedMsgingEventTransactedSessionCountThreshold))
                ) && 
                (
                    this.GuaranteedMsgingEventTransactedSessionResourceCountThreshold == input.GuaranteedMsgingEventTransactedSessionResourceCountThreshold ||
                    (this.GuaranteedMsgingEventTransactedSessionResourceCountThreshold != null &&
                    this.GuaranteedMsgingEventTransactedSessionResourceCountThreshold.Equals(input.GuaranteedMsgingEventTransactedSessionResourceCountThreshold))
                ) && 
                (
                    this.GuaranteedMsgingEventTransactionCountThreshold == input.GuaranteedMsgingEventTransactionCountThreshold ||
                    (this.GuaranteedMsgingEventTransactionCountThreshold != null &&
                    this.GuaranteedMsgingEventTransactionCountThreshold.Equals(input.GuaranteedMsgingEventTransactionCountThreshold))
                ) && 
                (
                    this.GuaranteedMsgingMaxCacheUsage == input.GuaranteedMsgingMaxCacheUsage ||
                    (this.GuaranteedMsgingMaxCacheUsage != null &&
                    this.GuaranteedMsgingMaxCacheUsage.Equals(input.GuaranteedMsgingMaxCacheUsage))
                ) && 
                (
                    this.GuaranteedMsgingMaxMsgSpoolUsage == input.GuaranteedMsgingMaxMsgSpoolUsage ||
                    (this.GuaranteedMsgingMaxMsgSpoolUsage != null &&
                    this.GuaranteedMsgingMaxMsgSpoolUsage.Equals(input.GuaranteedMsgingMaxMsgSpoolUsage))
                ) && 
                (
                    this.GuaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout == input.GuaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout ||
                    (this.GuaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout != null &&
                    this.GuaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout.Equals(input.GuaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout))
                ) && 
                (
                    this.GuaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout == input.GuaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout ||
                    (this.GuaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout != null &&
                    this.GuaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout.Equals(input.GuaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout))
                ) && 
                (
                    this.GuaranteedMsgingTransactionReplicationCompatibilityMode == input.GuaranteedMsgingTransactionReplicationCompatibilityMode ||
                    (this.GuaranteedMsgingTransactionReplicationCompatibilityMode != null &&
                    this.GuaranteedMsgingTransactionReplicationCompatibilityMode.Equals(input.GuaranteedMsgingTransactionReplicationCompatibilityMode))
                ) && 
                (
                    this.OauthProfileDefault == input.OauthProfileDefault ||
                    (this.OauthProfileDefault != null &&
                    this.OauthProfileDefault.Equals(input.OauthProfileDefault))
                ) && 
                (
                    this.ServiceAmqpEnabled == input.ServiceAmqpEnabled ||
                    (this.ServiceAmqpEnabled != null &&
                    this.ServiceAmqpEnabled.Equals(input.ServiceAmqpEnabled))
                ) && 
                (
                    this.ServiceAmqpTlsListenPort == input.ServiceAmqpTlsListenPort ||
                    (this.ServiceAmqpTlsListenPort != null &&
                    this.ServiceAmqpTlsListenPort.Equals(input.ServiceAmqpTlsListenPort))
                ) && 
                (
                    this.ServiceEventConnectionCountThreshold == input.ServiceEventConnectionCountThreshold ||
                    (this.ServiceEventConnectionCountThreshold != null &&
                    this.ServiceEventConnectionCountThreshold.Equals(input.ServiceEventConnectionCountThreshold))
                ) && 
                (
                    this.ServiceHealthCheckEnabled == input.ServiceHealthCheckEnabled ||
                    (this.ServiceHealthCheckEnabled != null &&
                    this.ServiceHealthCheckEnabled.Equals(input.ServiceHealthCheckEnabled))
                ) && 
                (
                    this.ServiceHealthCheckListenPort == input.ServiceHealthCheckListenPort ||
                    (this.ServiceHealthCheckListenPort != null &&
                    this.ServiceHealthCheckListenPort.Equals(input.ServiceHealthCheckListenPort))
                ) && 
                (
                    this.ServiceHealthCheckTlsEnabled == input.ServiceHealthCheckTlsEnabled ||
                    (this.ServiceHealthCheckTlsEnabled != null &&
                    this.ServiceHealthCheckTlsEnabled.Equals(input.ServiceHealthCheckTlsEnabled))
                ) && 
                (
                    this.ServiceHealthCheckTlsListenPort == input.ServiceHealthCheckTlsListenPort ||
                    (this.ServiceHealthCheckTlsListenPort != null &&
                    this.ServiceHealthCheckTlsListenPort.Equals(input.ServiceHealthCheckTlsListenPort))
                ) && 
                (
                    this.ServiceMateLinkEnabled == input.ServiceMateLinkEnabled ||
                    (this.ServiceMateLinkEnabled != null &&
                    this.ServiceMateLinkEnabled.Equals(input.ServiceMateLinkEnabled))
                ) && 
                (
                    this.ServiceMateLinkListenPort == input.ServiceMateLinkListenPort ||
                    (this.ServiceMateLinkListenPort != null &&
                    this.ServiceMateLinkListenPort.Equals(input.ServiceMateLinkListenPort))
                ) && 
                (
                    this.ServiceMqttEnabled == input.ServiceMqttEnabled ||
                    (this.ServiceMqttEnabled != null &&
                    this.ServiceMqttEnabled.Equals(input.ServiceMqttEnabled))
                ) && 
                (
                    this.ServiceMsgBackboneEnabled == input.ServiceMsgBackboneEnabled ||
                    (this.ServiceMsgBackboneEnabled != null &&
                    this.ServiceMsgBackboneEnabled.Equals(input.ServiceMsgBackboneEnabled))
                ) && 
                (
                    this.ServiceRedundancyEnabled == input.ServiceRedundancyEnabled ||
                    (this.ServiceRedundancyEnabled != null &&
                    this.ServiceRedundancyEnabled.Equals(input.ServiceRedundancyEnabled))
                ) && 
                (
                    this.ServiceRedundancyFirstListenPort == input.ServiceRedundancyFirstListenPort ||
                    (this.ServiceRedundancyFirstListenPort != null &&
                    this.ServiceRedundancyFirstListenPort.Equals(input.ServiceRedundancyFirstListenPort))
                ) && 
                (
                    this.ServiceRestEventOutgoingConnectionCountThreshold == input.ServiceRestEventOutgoingConnectionCountThreshold ||
                    (this.ServiceRestEventOutgoingConnectionCountThreshold != null &&
                    this.ServiceRestEventOutgoingConnectionCountThreshold.Equals(input.ServiceRestEventOutgoingConnectionCountThreshold))
                ) && 
                (
                    this.ServiceRestIncomingEnabled == input.ServiceRestIncomingEnabled ||
                    (this.ServiceRestIncomingEnabled != null &&
                    this.ServiceRestIncomingEnabled.Equals(input.ServiceRestIncomingEnabled))
                ) && 
                (
                    this.ServiceRestOutgoingEnabled == input.ServiceRestOutgoingEnabled ||
                    (this.ServiceRestOutgoingEnabled != null &&
                    this.ServiceRestOutgoingEnabled.Equals(input.ServiceRestOutgoingEnabled))
                ) && 
                (
                    this.ServiceSempCorsAllowAnyHostEnabled == input.ServiceSempCorsAllowAnyHostEnabled ||
                    (this.ServiceSempCorsAllowAnyHostEnabled != null &&
                    this.ServiceSempCorsAllowAnyHostEnabled.Equals(input.ServiceSempCorsAllowAnyHostEnabled))
                ) && 
                (
                    this.ServiceSempLegacyTimeoutEnabled == input.ServiceSempLegacyTimeoutEnabled ||
                    (this.ServiceSempLegacyTimeoutEnabled != null &&
                    this.ServiceSempLegacyTimeoutEnabled.Equals(input.ServiceSempLegacyTimeoutEnabled))
                ) && 
                (
                    this.ServiceSempPlainTextEnabled == input.ServiceSempPlainTextEnabled ||
                    (this.ServiceSempPlainTextEnabled != null &&
                    this.ServiceSempPlainTextEnabled.Equals(input.ServiceSempPlainTextEnabled))
                ) && 
                (
                    this.ServiceSempPlainTextListenPort == input.ServiceSempPlainTextListenPort ||
                    (this.ServiceSempPlainTextListenPort != null &&
                    this.ServiceSempPlainTextListenPort.Equals(input.ServiceSempPlainTextListenPort))
                ) && 
                (
                    this.ServiceSempSessionIdleTimeout == input.ServiceSempSessionIdleTimeout ||
                    (this.ServiceSempSessionIdleTimeout != null &&
                    this.ServiceSempSessionIdleTimeout.Equals(input.ServiceSempSessionIdleTimeout))
                ) && 
                (
                    this.ServiceSempSessionMaxLifetime == input.ServiceSempSessionMaxLifetime ||
                    (this.ServiceSempSessionMaxLifetime != null &&
                    this.ServiceSempSessionMaxLifetime.Equals(input.ServiceSempSessionMaxLifetime))
                ) && 
                (
                    this.ServiceSempTlsEnabled == input.ServiceSempTlsEnabled ||
                    (this.ServiceSempTlsEnabled != null &&
                    this.ServiceSempTlsEnabled.Equals(input.ServiceSempTlsEnabled))
                ) && 
                (
                    this.ServiceSempTlsListenPort == input.ServiceSempTlsListenPort ||
                    (this.ServiceSempTlsListenPort != null &&
                    this.ServiceSempTlsListenPort.Equals(input.ServiceSempTlsListenPort))
                ) && 
                (
                    this.ServiceSmfCompressionListenPort == input.ServiceSmfCompressionListenPort ||
                    (this.ServiceSmfCompressionListenPort != null &&
                    this.ServiceSmfCompressionListenPort.Equals(input.ServiceSmfCompressionListenPort))
                ) && 
                (
                    this.ServiceSmfEnabled == input.ServiceSmfEnabled ||
                    (this.ServiceSmfEnabled != null &&
                    this.ServiceSmfEnabled.Equals(input.ServiceSmfEnabled))
                ) && 
                (
                    this.ServiceSmfEventConnectionCountThreshold == input.ServiceSmfEventConnectionCountThreshold ||
                    (this.ServiceSmfEventConnectionCountThreshold != null &&
                    this.ServiceSmfEventConnectionCountThreshold.Equals(input.ServiceSmfEventConnectionCountThreshold))
                ) && 
                (
                    this.ServiceSmfPlainTextListenPort == input.ServiceSmfPlainTextListenPort ||
                    (this.ServiceSmfPlainTextListenPort != null &&
                    this.ServiceSmfPlainTextListenPort.Equals(input.ServiceSmfPlainTextListenPort))
                ) && 
                (
                    this.ServiceSmfRoutingControlListenPort == input.ServiceSmfRoutingControlListenPort ||
                    (this.ServiceSmfRoutingControlListenPort != null &&
                    this.ServiceSmfRoutingControlListenPort.Equals(input.ServiceSmfRoutingControlListenPort))
                ) && 
                (
                    this.ServiceSmfTlsListenPort == input.ServiceSmfTlsListenPort ||
                    (this.ServiceSmfTlsListenPort != null &&
                    this.ServiceSmfTlsListenPort.Equals(input.ServiceSmfTlsListenPort))
                ) && 
                (
                    this.ServiceTlsEventConnectionCountThreshold == input.ServiceTlsEventConnectionCountThreshold ||
                    (this.ServiceTlsEventConnectionCountThreshold != null &&
                    this.ServiceTlsEventConnectionCountThreshold.Equals(input.ServiceTlsEventConnectionCountThreshold))
                ) && 
                (
                    this.ServiceWebTransportEnabled == input.ServiceWebTransportEnabled ||
                    (this.ServiceWebTransportEnabled != null &&
                    this.ServiceWebTransportEnabled.Equals(input.ServiceWebTransportEnabled))
                ) && 
                (
                    this.ServiceWebTransportPlainTextListenPort == input.ServiceWebTransportPlainTextListenPort ||
                    (this.ServiceWebTransportPlainTextListenPort != null &&
                    this.ServiceWebTransportPlainTextListenPort.Equals(input.ServiceWebTransportPlainTextListenPort))
                ) && 
                (
                    this.ServiceWebTransportTlsListenPort == input.ServiceWebTransportTlsListenPort ||
                    (this.ServiceWebTransportTlsListenPort != null &&
                    this.ServiceWebTransportTlsListenPort.Equals(input.ServiceWebTransportTlsListenPort))
                ) && 
                (
                    this.ServiceWebTransportWebUrlSuffix == input.ServiceWebTransportWebUrlSuffix ||
                    (this.ServiceWebTransportWebUrlSuffix != null &&
                    this.ServiceWebTransportWebUrlSuffix.Equals(input.ServiceWebTransportWebUrlSuffix))
                ) && 
                (
                    this.TlsBlockVersion11Enabled == input.TlsBlockVersion11Enabled ||
                    (this.TlsBlockVersion11Enabled != null &&
                    this.TlsBlockVersion11Enabled.Equals(input.TlsBlockVersion11Enabled))
                ) && 
                (
                    this.TlsCipherSuiteManagementList == input.TlsCipherSuiteManagementList ||
                    (this.TlsCipherSuiteManagementList != null &&
                    this.TlsCipherSuiteManagementList.Equals(input.TlsCipherSuiteManagementList))
                ) && 
                (
                    this.TlsCipherSuiteMsgBackboneList == input.TlsCipherSuiteMsgBackboneList ||
                    (this.TlsCipherSuiteMsgBackboneList != null &&
                    this.TlsCipherSuiteMsgBackboneList.Equals(input.TlsCipherSuiteMsgBackboneList))
                ) && 
                (
                    this.TlsCipherSuiteSecureShellList == input.TlsCipherSuiteSecureShellList ||
                    (this.TlsCipherSuiteSecureShellList != null &&
                    this.TlsCipherSuiteSecureShellList.Equals(input.TlsCipherSuiteSecureShellList))
                ) && 
                (
                    this.TlsCrimeExploitProtectionEnabled == input.TlsCrimeExploitProtectionEnabled ||
                    (this.TlsCrimeExploitProtectionEnabled != null &&
                    this.TlsCrimeExploitProtectionEnabled.Equals(input.TlsCrimeExploitProtectionEnabled))
                ) && 
                (
                    this.TlsServerCertContent == input.TlsServerCertContent ||
                    (this.TlsServerCertContent != null &&
                    this.TlsServerCertContent.Equals(input.TlsServerCertContent))
                ) && 
                (
                    this.TlsServerCertPassword == input.TlsServerCertPassword ||
                    (this.TlsServerCertPassword != null &&
                    this.TlsServerCertPassword.Equals(input.TlsServerCertPassword))
                ) && 
                (
                    this.TlsStandardDomainCertificateAuthoritiesEnabled == input.TlsStandardDomainCertificateAuthoritiesEnabled ||
                    (this.TlsStandardDomainCertificateAuthoritiesEnabled != null &&
                    this.TlsStandardDomainCertificateAuthoritiesEnabled.Equals(input.TlsStandardDomainCertificateAuthoritiesEnabled))
                ) && 
                (
                    this.TlsTicketLifetime == input.TlsTicketLifetime ||
                    (this.TlsTicketLifetime != null &&
                    this.TlsTicketLifetime.Equals(input.TlsTicketLifetime))
                ) && 
                (
                    this.WebManagerAllowUnencryptedWizardsEnabled == input.WebManagerAllowUnencryptedWizardsEnabled ||
                    (this.WebManagerAllowUnencryptedWizardsEnabled != null &&
                    this.WebManagerAllowUnencryptedWizardsEnabled.Equals(input.WebManagerAllowUnencryptedWizardsEnabled))
                ) && 
                (
                    this.WebManagerCustomization == input.WebManagerCustomization ||
                    (this.WebManagerCustomization != null &&
                    this.WebManagerCustomization.Equals(input.WebManagerCustomization))
                ) && 
                (
                    this.WebManagerRedirectHttpEnabled == input.WebManagerRedirectHttpEnabled ||
                    (this.WebManagerRedirectHttpEnabled != null &&
                    this.WebManagerRedirectHttpEnabled.Equals(input.WebManagerRedirectHttpEnabled))
                ) && 
                (
                    this.WebManagerRedirectHttpOverrideTlsPort == input.WebManagerRedirectHttpOverrideTlsPort ||
                    (this.WebManagerRedirectHttpOverrideTlsPort != null &&
                    this.WebManagerRedirectHttpOverrideTlsPort.Equals(input.WebManagerRedirectHttpOverrideTlsPort))
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
                if (this.AuthClientCertRevocationCheckMode != null)
                    hashCode = hashCode * 59 + this.AuthClientCertRevocationCheckMode.GetHashCode();
                if (this.ConfigSyncAuthenticationClientCertMaxChainDepth != null)
                    hashCode = hashCode * 59 + this.ConfigSyncAuthenticationClientCertMaxChainDepth.GetHashCode();
                if (this.ConfigSyncAuthenticationClientCertValidateDateEnabled != null)
                    hashCode = hashCode * 59 + this.ConfigSyncAuthenticationClientCertValidateDateEnabled.GetHashCode();
                if (this.ConfigSyncClientProfileTcpInitialCongestionWindow != null)
                    hashCode = hashCode * 59 + this.ConfigSyncClientProfileTcpInitialCongestionWindow.GetHashCode();
                if (this.ConfigSyncClientProfileTcpKeepaliveCount != null)
                    hashCode = hashCode * 59 + this.ConfigSyncClientProfileTcpKeepaliveCount.GetHashCode();
                if (this.ConfigSyncClientProfileTcpKeepaliveIdle != null)
                    hashCode = hashCode * 59 + this.ConfigSyncClientProfileTcpKeepaliveIdle.GetHashCode();
                if (this.ConfigSyncClientProfileTcpKeepaliveInterval != null)
                    hashCode = hashCode * 59 + this.ConfigSyncClientProfileTcpKeepaliveInterval.GetHashCode();
                if (this.ConfigSyncClientProfileTcpMaxWindow != null)
                    hashCode = hashCode * 59 + this.ConfigSyncClientProfileTcpMaxWindow.GetHashCode();
                if (this.ConfigSyncClientProfileTcpMss != null)
                    hashCode = hashCode * 59 + this.ConfigSyncClientProfileTcpMss.GetHashCode();
                if (this.ConfigSyncEnabled != null)
                    hashCode = hashCode * 59 + this.ConfigSyncEnabled.GetHashCode();
                if (this.ConfigSyncSynchronizeUsernameEnabled != null)
                    hashCode = hashCode * 59 + this.ConfigSyncSynchronizeUsernameEnabled.GetHashCode();
                if (this.ConfigSyncTlsEnabled != null)
                    hashCode = hashCode * 59 + this.ConfigSyncTlsEnabled.GetHashCode();
                if (this.GuaranteedMsgingDefragmentationScheduleDayList != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingDefragmentationScheduleDayList.GetHashCode();
                if (this.GuaranteedMsgingDefragmentationScheduleEnabled != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingDefragmentationScheduleEnabled.GetHashCode();
                if (this.GuaranteedMsgingDefragmentationScheduleTimeList != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingDefragmentationScheduleTimeList.GetHashCode();
                if (this.GuaranteedMsgingDefragmentationThresholdEnabled != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingDefragmentationThresholdEnabled.GetHashCode();
                if (this.GuaranteedMsgingDefragmentationThresholdFragmentationPercentage != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingDefragmentationThresholdFragmentationPercentage.GetHashCode();
                if (this.GuaranteedMsgingDefragmentationThresholdMinInterval != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingDefragmentationThresholdMinInterval.GetHashCode();
                if (this.GuaranteedMsgingDefragmentationThresholdUsagePercentage != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingDefragmentationThresholdUsagePercentage.GetHashCode();
                if (this.GuaranteedMsgingEnabled != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingEnabled.GetHashCode();
                if (this.GuaranteedMsgingEventCacheUsageThreshold != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingEventCacheUsageThreshold.GetHashCode();
                if (this.GuaranteedMsgingEventDeliveredUnackedThreshold != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingEventDeliveredUnackedThreshold.GetHashCode();
                if (this.GuaranteedMsgingEventDiskUsageThreshold != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingEventDiskUsageThreshold.GetHashCode();
                if (this.GuaranteedMsgingEventEgressFlowCountThreshold != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingEventEgressFlowCountThreshold.GetHashCode();
                if (this.GuaranteedMsgingEventEndpointCountThreshold != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingEventEndpointCountThreshold.GetHashCode();
                if (this.GuaranteedMsgingEventIngressFlowCountThreshold != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingEventIngressFlowCountThreshold.GetHashCode();
                if (this.GuaranteedMsgingEventMsgCountThreshold != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingEventMsgCountThreshold.GetHashCode();
                if (this.GuaranteedMsgingEventMsgSpoolFileCountThreshold != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingEventMsgSpoolFileCountThreshold.GetHashCode();
                if (this.GuaranteedMsgingEventMsgSpoolUsageThreshold != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingEventMsgSpoolUsageThreshold.GetHashCode();
                if (this.GuaranteedMsgingEventTransactedSessionCountThreshold != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingEventTransactedSessionCountThreshold.GetHashCode();
                if (this.GuaranteedMsgingEventTransactedSessionResourceCountThreshold != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingEventTransactedSessionResourceCountThreshold.GetHashCode();
                if (this.GuaranteedMsgingEventTransactionCountThreshold != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingEventTransactionCountThreshold.GetHashCode();
                if (this.GuaranteedMsgingMaxCacheUsage != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingMaxCacheUsage.GetHashCode();
                if (this.GuaranteedMsgingMaxMsgSpoolUsage != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingMaxMsgSpoolUsage.GetHashCode();
                if (this.GuaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingMsgSpoolSyncMirroredMsgAckTimeout.GetHashCode();
                if (this.GuaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingMsgSpoolSyncMirroredSpoolFileAckTimeout.GetHashCode();
                if (this.GuaranteedMsgingTransactionReplicationCompatibilityMode != null)
                    hashCode = hashCode * 59 + this.GuaranteedMsgingTransactionReplicationCompatibilityMode.GetHashCode();
                if (this.OauthProfileDefault != null)
                    hashCode = hashCode * 59 + this.OauthProfileDefault.GetHashCode();
                if (this.ServiceAmqpEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceAmqpEnabled.GetHashCode();
                if (this.ServiceAmqpTlsListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceAmqpTlsListenPort.GetHashCode();
                if (this.ServiceEventConnectionCountThreshold != null)
                    hashCode = hashCode * 59 + this.ServiceEventConnectionCountThreshold.GetHashCode();
                if (this.ServiceHealthCheckEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceHealthCheckEnabled.GetHashCode();
                if (this.ServiceHealthCheckListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceHealthCheckListenPort.GetHashCode();
                if (this.ServiceHealthCheckTlsEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceHealthCheckTlsEnabled.GetHashCode();
                if (this.ServiceHealthCheckTlsListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceHealthCheckTlsListenPort.GetHashCode();
                if (this.ServiceMateLinkEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceMateLinkEnabled.GetHashCode();
                if (this.ServiceMateLinkListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceMateLinkListenPort.GetHashCode();
                if (this.ServiceMqttEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceMqttEnabled.GetHashCode();
                if (this.ServiceMsgBackboneEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceMsgBackboneEnabled.GetHashCode();
                if (this.ServiceRedundancyEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceRedundancyEnabled.GetHashCode();
                if (this.ServiceRedundancyFirstListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceRedundancyFirstListenPort.GetHashCode();
                if (this.ServiceRestEventOutgoingConnectionCountThreshold != null)
                    hashCode = hashCode * 59 + this.ServiceRestEventOutgoingConnectionCountThreshold.GetHashCode();
                if (this.ServiceRestIncomingEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceRestIncomingEnabled.GetHashCode();
                if (this.ServiceRestOutgoingEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceRestOutgoingEnabled.GetHashCode();
                if (this.ServiceSempCorsAllowAnyHostEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceSempCorsAllowAnyHostEnabled.GetHashCode();
                if (this.ServiceSempLegacyTimeoutEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceSempLegacyTimeoutEnabled.GetHashCode();
                if (this.ServiceSempPlainTextEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceSempPlainTextEnabled.GetHashCode();
                if (this.ServiceSempPlainTextListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceSempPlainTextListenPort.GetHashCode();
                if (this.ServiceSempSessionIdleTimeout != null)
                    hashCode = hashCode * 59 + this.ServiceSempSessionIdleTimeout.GetHashCode();
                if (this.ServiceSempSessionMaxLifetime != null)
                    hashCode = hashCode * 59 + this.ServiceSempSessionMaxLifetime.GetHashCode();
                if (this.ServiceSempTlsEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceSempTlsEnabled.GetHashCode();
                if (this.ServiceSempTlsListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceSempTlsListenPort.GetHashCode();
                if (this.ServiceSmfCompressionListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceSmfCompressionListenPort.GetHashCode();
                if (this.ServiceSmfEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceSmfEnabled.GetHashCode();
                if (this.ServiceSmfEventConnectionCountThreshold != null)
                    hashCode = hashCode * 59 + this.ServiceSmfEventConnectionCountThreshold.GetHashCode();
                if (this.ServiceSmfPlainTextListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceSmfPlainTextListenPort.GetHashCode();
                if (this.ServiceSmfRoutingControlListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceSmfRoutingControlListenPort.GetHashCode();
                if (this.ServiceSmfTlsListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceSmfTlsListenPort.GetHashCode();
                if (this.ServiceTlsEventConnectionCountThreshold != null)
                    hashCode = hashCode * 59 + this.ServiceTlsEventConnectionCountThreshold.GetHashCode();
                if (this.ServiceWebTransportEnabled != null)
                    hashCode = hashCode * 59 + this.ServiceWebTransportEnabled.GetHashCode();
                if (this.ServiceWebTransportPlainTextListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceWebTransportPlainTextListenPort.GetHashCode();
                if (this.ServiceWebTransportTlsListenPort != null)
                    hashCode = hashCode * 59 + this.ServiceWebTransportTlsListenPort.GetHashCode();
                if (this.ServiceWebTransportWebUrlSuffix != null)
                    hashCode = hashCode * 59 + this.ServiceWebTransportWebUrlSuffix.GetHashCode();
                if (this.TlsBlockVersion11Enabled != null)
                    hashCode = hashCode * 59 + this.TlsBlockVersion11Enabled.GetHashCode();
                if (this.TlsCipherSuiteManagementList != null)
                    hashCode = hashCode * 59 + this.TlsCipherSuiteManagementList.GetHashCode();
                if (this.TlsCipherSuiteMsgBackboneList != null)
                    hashCode = hashCode * 59 + this.TlsCipherSuiteMsgBackboneList.GetHashCode();
                if (this.TlsCipherSuiteSecureShellList != null)
                    hashCode = hashCode * 59 + this.TlsCipherSuiteSecureShellList.GetHashCode();
                if (this.TlsCrimeExploitProtectionEnabled != null)
                    hashCode = hashCode * 59 + this.TlsCrimeExploitProtectionEnabled.GetHashCode();
                if (this.TlsServerCertContent != null)
                    hashCode = hashCode * 59 + this.TlsServerCertContent.GetHashCode();
                if (this.TlsServerCertPassword != null)
                    hashCode = hashCode * 59 + this.TlsServerCertPassword.GetHashCode();
                if (this.TlsStandardDomainCertificateAuthoritiesEnabled != null)
                    hashCode = hashCode * 59 + this.TlsStandardDomainCertificateAuthoritiesEnabled.GetHashCode();
                if (this.TlsTicketLifetime != null)
                    hashCode = hashCode * 59 + this.TlsTicketLifetime.GetHashCode();
                if (this.WebManagerAllowUnencryptedWizardsEnabled != null)
                    hashCode = hashCode * 59 + this.WebManagerAllowUnencryptedWizardsEnabled.GetHashCode();
                if (this.WebManagerCustomization != null)
                    hashCode = hashCode * 59 + this.WebManagerCustomization.GetHashCode();
                if (this.WebManagerRedirectHttpEnabled != null)
                    hashCode = hashCode * 59 + this.WebManagerRedirectHttpEnabled.GetHashCode();
                if (this.WebManagerRedirectHttpOverrideTlsPort != null)
                    hashCode = hashCode * 59 + this.WebManagerRedirectHttpOverrideTlsPort.GetHashCode();
                return hashCode;
            }
        }
    }
}
