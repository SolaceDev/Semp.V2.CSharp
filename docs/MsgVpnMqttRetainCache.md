# Semp.V2.CSharp.Model.MsgVpnMqttRetainCache
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**CacheName** | **string** | The name of the MQTT Retain Cache. | [optional] 
**Enabled** | **bool?** | Enable or disable this MQTT Retain Cache. When the cache is disabled, neither retain messages nor retain requests will be delivered by the cache. However, live retain messages will continue to be delivered to currently connected MQTT clients. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. | [optional] 
**MsgLifetime** | **long?** | The message lifetime, in seconds. If a message remains cached for the duration of its lifetime, the cache will remove the message. A lifetime of 0 results in the message being retained indefinitely, otherwise it must be 3 seconds or more. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;0&#x60;. | [optional] 
**MsgVpnName** | **string** | The name of the Message VPN. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

