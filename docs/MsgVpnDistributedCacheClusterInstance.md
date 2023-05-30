# Semp.V2.CSharp.Model.MsgVpnDistributedCacheClusterInstance
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**AutoStartEnabled** | **bool?** | Enable or disable auto-start for the Cache Instance. When enabled, the Cache Instance will automatically attempt to transition from the Stopped operational state to Up whenever it restarts or reconnects to the message broker. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. | [optional] 
**CacheName** | **string** | The name of the Distributed Cache. | [optional] 
**ClusterName** | **string** | The name of the Cache Cluster. | [optional] 
**Enabled** | **bool?** | Enable or disable the Cache Instance. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. | [optional] 
**InstanceName** | **string** | The name of the Cache Instance. | [optional] 
**MsgVpnName** | **string** | The name of the Message VPN. | [optional] 
**StopOnLostMsgEnabled** | **bool?** | Enable or disable stop-on-lost-message for the Cache Instance. When enabled, the Cache Instance will transition to the stopped operational state upon losing a message. When stopped, it cannot accept or respond to cache requests, but continues to cache messages. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;true&#x60;. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

