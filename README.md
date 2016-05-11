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

		InjectedAllowedTypes.RegisterInjectedAllowedTypesAttributes(new Dictionary<string, InjectedAllowedTypesAttribute> {
		{
			string.Format("{0}.{1}", typeof(MediaPage).Name, nameof(MediaPage.ContentArea)), new InjectedAllowedTypesAttribute {
				AllowedTypes = new[] { typeof(MusicBlock) }
			}
		}, {
			string.Format("{0}.{1}", typeof(MediaPage).Name, nameof(MediaPage.Items)), new InjectedAllowedTypesAttribute {
				AllowedTypes = new[] { typeof(CoolPage) }
			}
		}, {
			string.Format("{0}.{1}", typeof(MediaPage).Name, nameof(MediaPage.ContentReference)), new InjectedAllowedTypesAttribute {
				AllowedTypes = new[] { typeof(CoolPage) }
			}
		}});
	}

	public void Uninitialize(InitializationEngine context) {}

	public void Preload(string[] parameters) {}
}
```
