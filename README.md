Example usage
```csharp
[InitializableModule]
[ModuleDependency(typeof(EPiServer.Shell.ShellInitialization))]
public class InjectedAllowedTypesInitialization : IInitializableModule {
	public void Initialize(InitializationEngine context) {
		InjectedAvailableModelSettings.RegisterCustomAvailableModelSettings(new Dictionary<Type, ContentTypeAvailableModelSetting> {
		{
			typeof(MediaPage), new ContentTypeAvailableModelSetting {
				Availability = Availability.Specific,
				Included = new HashSet<Type> { typeof(CoolPage) }
			}
		}});

		InjectedAllowedTypes.RegisterInjectedAllowedTypesAttributes(new Dictionary<string, AllowedTypesAttribute> {
		{
			string.Format("{0}.{1}", typeof(MediaPage).Name, "ContentArea"), new AllowedTypesAttribute {
				AllowedTypes = new[] { typeof(MusicBlock) }
			}
		}, {
			string.Format("{0}.{1}", typeof(MediaPage).Name, "Items"), new AllowedTypesAttribute {
				AllowedTypes = new[] { typeof(CoolPage) }
			}
		}, {
			string.Format("{0}.{1}", typeof(MediaPage).Name, "ContentReference"), new AllowedTypesAttribute {
				AllowedTypes = new[] { typeof(CoolPage) }
			}
		}});
	}

	public void Uninitialize(InitializationEngine context) {}

	public void Preload(string[] parameters) {}
}
```
