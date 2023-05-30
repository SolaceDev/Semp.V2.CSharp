# Semp.V2.CSharp.Model.MsgVpnTelemetryProfileTraceFilter
## Properties

Name | Type | Description | Notes
------------ | ------------- | ------------- | -------------
**Enabled** | **bool?** | Enable or disable the trace filter. When the filter is disabled, the filter&#x27;s subscriptions will not trigger a message to be traced. Changes to this attribute are synchronized to HA mates and replication sites via config-sync. The default value is &#x60;false&#x60;. | [optional] 
**MsgVpnName** | **string** | The name of the Message VPN. | [optional] 
**TelemetryProfileName** | **string** | The name of the Telemetry Profile. | [optional] 
**TraceFilterName** | **string** | A name used to identify the trace filter. Consider a name that describes the subscriptions contained within the filter, such as the name of the application and/or the scenario in which the trace filter might be enabled, such as \&quot;appNameDebug\&quot;. | [optional] 

[[Back to Model list]](../README.md#documentation-for-models) [[Back to API list]](../README.md#documentation-for-api-endpoints) [[Back to README]](../README.md)

