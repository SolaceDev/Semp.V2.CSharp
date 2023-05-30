# Semp.V2.CSharp.Model.MsgVpnMqttSessionSubscription
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**MqttSessionClientId** | **string** | The Client ID of the MQTT Session, which corresponds to the ClientId provided in the MQTT CONNECT packet. | [optional] 
**MqttSessionVirtualRouter** | **string** | The virtual router of the MQTT Session. The allowed values and their meaning are:  &lt;pre&gt; \&quot;primary\&quot; - The MQTT Session belongs to the primary virtual router. \&quot;backup\&quot; - The MQTT Session belongs to the backup virtual router. \&quot;auto\&quot; - The MQTT Session is automatically assigned a virtual router at creation, depending on the broker&#x27;s active-standby role. &lt;/pre&gt;  | [optional] 
**MsgVpnName** | **string** | The name of the Message VPN. | [optional] 
**SubscriptionQos** | **long?** | The quality of service (QoS) for the subscription as either 0 (deliver at most once) or 1 (deliver at least once). QoS 2 is not supported, but QoS 2 messages attracted by QoS 0 or QoS 1 subscriptions are accepted and delivered accordingly. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. | [optional] 
**SubscriptionTopic** | **string** | The MQTT subscription topic. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

