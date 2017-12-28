# Breaking Changes

- **Renaming HoloToolkit-Unity repository to MixedRealityToolkit-Unity** to align with product direction.
- Technically **all your checkins and redirect links will continue to work as-is** but we wanted to give a better heads up on this.
- All other dependent repositories will undergo a similar name change.
- We are **not breaking toolkit folder names and namespaces at this time.**
- Instead we are taking a staggered approach for breaking changes based on developer feedback.

| Breaking change description | Notes |
| --- |  --- |
| Rename repository to MixedRealityToolkit-Unity. | <ul><li>Recommend you do: $git remote set-url origin new_url.</li><li>Recommend reading: https://help.github.com/articles/renaming-a-repository ; https://github.com/blog/1508-repository-redirects-are-here</li></ul>|
| Updating toolkit namespace to MixedReality      |  <ul><li>Update folder names, class names and namespace names post 2017.2.0 release.</li></ul>|