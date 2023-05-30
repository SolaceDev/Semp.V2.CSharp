# Semp.V2.CSharp.Model.MsgVpnTelemetryProfile
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**MsgVpnName** | **string** | The name of the Message VPN. | [optional] 
**QueueEventBindCountThreshold** | [**EventThreshold**](EventThreshold.md) |  | [optional] 
**QueueEventMsgSpoolUsageThreshold** | [**EventThreshold**](EventThreshold.md) |  | [optional] 
**QueueMaxBindCount** | **long?** | The maximum number of consumer flows that can bind to the Queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1000&#x60;. | [optional] 
**QueueMaxMsgSpoolUsage** | **long?** | The maximum message spool usage allowed by the Queue, in megabytes (MB). Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;800000&#x60;. | [optional] 
**ReceiverAclConnectDefaultAction** | **string** | The default action to take when a receiver client connects to the broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;\&quot;disallow\&quot;&#x60;. The allowed values and their meaning are:  &lt;pre&gt; \&quot;allow\&quot; - Allow client connection unless an exception is found for it. \&quot;disallow\&quot; - Disallow client connection unless an exception is found for it. &lt;/pre&gt;  | [optional] 
**ReceiverEnabled** | **bool?** | Enable or disable the ability for receiver clients to consume from the #telemetry queue. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. | [optional] 
**ReceiverEventConnectionCountPerClientUsernameThreshold** | [**EventThreshold**](EventThreshold.md) |  | [optional] 
**ReceiverMaxConnectionCountPerClientUsername** | **long?** | The maximum number of receiver connections per Client Username. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default is the maximum value supported by the platform. | [optional] 
**ReceiverTcpCongestionWindowSize** | **long?** | The TCP initial congestion window size for clients using the Client Profile, in multiples of the TCP Maximum Segment Size (MSS). Changing the value from its default of 2 results in non-compliance with RFC 2581. Contact support before changing this value. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;2&#x60;. | [optional] 
**ReceiverTcpKeepaliveCount** | **long?** | The number of TCP keepalive retransmissions to a client using the Client Profile before declaring that it is not available. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;5&#x60;. | [optional] 
**ReceiverTcpKeepaliveIdleTime** | **long?** | The amount of time a client connection using the Client Profile must remain idle before TCP begins sending keepalive probes, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;3&#x60;. | [optional] 
**ReceiverTcpKeepaliveInterval** | **long?** | The amount of time between TCP keepalive retransmissions to a client using the Client Profile when no acknowledgement is received, in seconds. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1&#x60;. | [optional] 
**ReceiverTcpMaxSegmentSize** | **long?** | The TCP maximum segment size for clients using the Client Profile, in bytes. Changes are applied to all existing connections. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;1460&#x60;. | [optional] 
**ReceiverTcpMaxWindowSize** | **long?** | The TCP maximum window size for clients using the Client Profile, in kilobytes. Changes are applied to all existing connections. This setting is ignored on the software broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;256&#x60;. | [optional] 
**TelemetryProfileName** | **string** | The name of the Telemetry Profile. | [optional] 
**TraceEnabled** | **bool?** | Enable or disable generation of all trace span data messages. When enabled, the state of configured trace filters control which messages get traced. When disabled, trace span data messages are never generated, regardless of the state of trace filters. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

