# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project

`CommonControls` is a reusable Windows Forms custom-control library (class library, `OutputType=Library`) targeting **.NET Framework 4.8**. It has no dependencies beyond the BCL/WinForms references already listed in the csproj. It ships as a single `CommonControls.dll` that other WinForms solutions reference and drop onto the designer toolbox.

## Build

```powershell
dotnet build CommonControls.sln -p:Configuration=Release
```

Full MSBuild also works if the SDK resolution ever misbehaves on the legacy project format:

```powershell
& "C:\Program Files\Microsoft Visual Studio\18\Enterprise\MSBuild\Current\Bin\MSBuild.exe" CommonControls.sln -p:Configuration=Release
```

Output lands in `bin\Debug\` / `bin\Release\`; a `PostBuildEvent` in the csproj additionally `xcopy`s the built DLL up to `bin\` so consumers can reference one stable path regardless of configuration.

There are no tests, no linter, and no CI. Verification is manual: build the library, then drop the control on a WinForms form and exercise its states (on/off/disabled, both drawing styles) in the designer or at runtime.

## Project format

This is a **legacy (non-SDK-style) csproj**. New source files are *not* picked up automatically — every `.cs` file must be added by hand to the `<ItemGroup>` in `CommonControls.csproj`. Control classes carry `<SubType>Component</SubType>` so the WinForms designer treats them as designable components:

```xml
<Compile Include="MyControl.cs">
  <SubType>Component</SubType>
</Compile>
```

Assembly version lives in `Properties\AssemblyInfo.cs` (`AssemblyVersion` + `AssemblyFileVersion`, kept in lockstep) — bump both there, not in the csproj.

## Control architecture

Controls derive from an existing WinForms control and take over rendering by overriding `OnPaint`, rather than composing child controls. `ToggleButton` is the reference implementation and the pattern to follow:

- **Derive for behavior, repaint for looks.** `ToggleButton : CheckBox` inherits click handling, `Checked` state, and data binding; only the drawing is replaced.
- **State → color pair.** `OnPaint` picks a `(background, toggle)` color pair from the control's state in a fixed priority order — disabled first, then checked, then unchecked — and everything downstream draws from that pair.
- **Geometry is computed, never stored.** Private helpers (`GetBackgroundPath`, `GetToggleRectangle`) derive all shapes from the live `Height`/`Width` at paint time, so the control scales with whatever size the designer gives it. A `MinimumSize` is set in the constructor to keep the math from degenerating.
- **Public appearance knobs are auto-properties over `m_`-prefixed fields, and every setter calls `this.Invalidate()`** so the designer and runtime repaint immediately. Use `[DefaultValue(...)]` on enum properties so the designer serializes them correctly.
- **Inapplicable inherited members are hidden by overriding away the setter** — e.g. `public override string Text { get => base.Text; }` on a control that never renders text.
- `OnPaint` clears with `this.Parent.BackColor`, so a control instance requires a parent at paint time.

## Code style

Match the existing file conventions exactly; they are deliberate and consistent:

- Every file opens with the banner comment block: file name, description, copyright, MIT license notice, and a dated **Revision History** table. Append a `YYYY/MM/DD - Name - description` line when meaningfully changing a file.
- Members are grouped in `#region` blocks in this order: `Type Definitions`, `Contructors and Destructor` (note the existing spelling), `Event Handlers`, `Methods`, `Properties`, `Data Members`. Private fields go last, not first.
- Private fields use `m_PascalCase`; local integers use a Hungarian `i` prefix (`iToggleRadius`).
- Yoda conditions for comparisons against constants: `if (false == this.Enabled)`, `if (DrawingStyles.Solid == m_Style)`.
- Nearly every statement or block gets a preceding `//` comment explaining intent, including the `// Otherwise, ...` comments on `else` branches.
- XML doc comments on all types and members; `<param>` text is prefixed with direction, e.g. `IN - The paint event arguments`.
