# CommonControls

A small library of custom [Windows Forms](https://learn.microsoft.com/dotnet/desktop/winforms/) controls for .NET Framework 4.8.

Each control derives from a stock WinForms control — inheriting its behavior, state, and data binding — and replaces only the rendering, so it drops into the designer and behaves exactly like the control it extends.

## Controls

### ToggleButton

An iOS-style sliding toggle switch. It derives from `CheckBox`, so `Checked`, `CheckedChanged`, click handling, and data binding all work as usual — it just draws as a pill-shaped track with a sliding knob instead of a box and a label.

**Properties**

| Property | Type | Default | Description |
| --- | --- | --- | --- |
| `OnBackground` | `Color` | `Black` | Track color when checked |
| `OnToggle` | `Color` | `White` | Knob color when checked |
| `OffBackground` | `Color` | `Black` | Track color when unchecked |
| `OffToggle` | `Color` | `White` | Knob color when unchecked |
| `DisabledBackground` | `Color` | `Gray` | Track color when disabled |
| `DisabledToggle` | `Color` | `LightGray` | Knob color when disabled |
| `Style` | `DrawingStyles` | `Solid` | `Solid` fills the track; `Hollow` draws only its outline |

Setting any of these repaints the control immediately, in the designer as well as at runtime.

**Notes**

- `Text` is read-only — the control draws no caption.
- A `MinimumSize` of 50 × 25 is enforced so the knob geometry stays correct.
- The control clears itself with its parent's `BackColor`, so it must be placed on a container before it paints.

**Usage**

```csharp
using CommonControls;

var toggle = new ToggleButton
{
    Location      = new Point(20, 20),
    Size          = new Size(60, 30),
    OnBackground  = Color.SeaGreen,
    OnToggle      = Color.White,
    OffBackground = Color.LightGray,
    OffToggle     = Color.DimGray,
    Style         = ToggleButton.DrawingStyles.Solid,
};

toggle.CheckedChanged += (s, e) => Console.WriteLine($"Toggle is {(toggle.Checked ? "on" : "off")}");
Controls.Add(toggle);
```

## Requirements

- .NET Framework 4.8
- Windows, with either the .NET SDK or Visual Studio (any edition with the .NET desktop development workload)

## Building

```powershell
dotnet build CommonControls.sln -p:Configuration=Release
```

Or open `CommonControls.sln` in Visual Studio and build.

The resulting `CommonControls.dll` is written to `bin\Release\`, alongside a `CommonControls.xml` documentation file, and a post-build step also copies the DLL to `bin\` so consumers can reference a single path regardless of configuration.

## Using the library

1. Download `CommonControls-<version>.zip` from the [latest release](https://github.com/Skorinn/CommonControls/releases/latest) and unpack it, or build the solution yourself.
2. In your WinForms project, add a reference to `CommonControls.dll`. Keep `CommonControls.xml` beside it to get IntelliSense for the control's properties.
3. Build your project once — the controls then appear in the Visual Studio toolbox and can be dragged onto a form and configured from the Properties window. Or create them in code, as shown above.

## Releasing

Releases are built and published by the [Release workflow](.github/workflows/release.yml) whenever a version tag is pushed:

1. Update `AssemblyVersion`, `AssemblyFileVersion`, and `AssemblyInformationalVersion` in `Properties\AssemblyInfo.cs`, then commit.
2. Tag the commit and push the tag:

```powershell
git tag v1.0.0
git push origin v1.0.0
```

The workflow refuses to publish if the tag does not match the assembly version. It builds the Release configuration, packages the DLL, the documentation file, `LICENSE`, and `README.md` into a zip, and attaches it to a new GitHub release. Running the workflow manually from the Actions tab builds and packages without publishing.

## License

Released under the MIT License. See [LICENSE](LICENSE) for the full text.

Copyright © 2023 Mike Pullen
