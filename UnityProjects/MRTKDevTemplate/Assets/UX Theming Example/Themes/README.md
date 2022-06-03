# UX Control Theming

All UXComponents controls can easily be themed. New UX elements that are application specific can also be themed quite easily by creating either a new binding profile, or extending the existing UXComponents binding profile. A single script, DataBindingConfigurator, added to the root of a prefab, will do all the necessary binding.

Any data source of any kind such as DataSourceReflection, DataSourceDictionary, or DataSourceJSON can be used to provide theming data. 

The default UXComponents controls uses a ScriptableObject, MRTK_UXComponents_ThemeProfile, as a data source, which is then attached to a DataSourceReflection data source instance to retrieve data from it. 

As long as the data source uses the same naming conventions as the UXComponents controls, as seen in the MRTK_UXComponents_ThemeProfile instance of the UXCoreThemeProfile ScriptableObject, it will work properly with the standard UXComponents controls.

Although not a requirement, a helper script called ThemeProvider, can be used to make it easy to set up and provide the right profile.  If this is higher in the Scene's GameObject heirarchy than the controls that depend on it, it will automatically be discovered and used because it implements IDataSourceProvider