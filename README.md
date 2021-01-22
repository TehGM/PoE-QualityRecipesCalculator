# Quality Recipes Calculator for Path of Exile
[![GitHub release (latest SemVer)](https://img.shields.io/github/v/release/TehGM/PoE-QualityRecipesCalculator)](https://github.com/TehGM/PoE-QualityRecipesCalculator/releases) [![GitHub top language](https://img.shields.io/github/languages/top/TehGM/PoE-QualityRecipesCalculator)](https://github.com/TehGM/PoE-QualityRecipesCalculator) [![GitHub](https://img.shields.io/github/license/TehGM/PoE-QualityRecipesCalculator)](LICENSE) [![GitHub Workflow Status](https://img.shields.io/github/workflow/status/TehGM/PoE-QualityRecipesCalculator/.NET%20Build)](https://github.com/TehGM/PoE-QualityRecipesCalculator/actions) [![GitHub issues](https://img.shields.io/github/issues/TehGM/PoE-QualityRecipesCalculator)](https://github.com/TehGM/PoE-QualityRecipesCalculator/issues)

This is a simple terminal application that will scan Stash tabs, and output a list of possible combinations for quality vendor recipes.

I made this application mostly for myself - I tend to hoard quality flasks and gems for vendor recipes, but only sell if I can get exact 40% quality sum. And I got tired of looking at my tab and trying to do math in my head. And I can program, so decided to make use of that.

This tool uses [PoE API](https://app.swaggerhub.com/apis-docs/Chuanhsing/poe/1.0.0) to get stash data.

### Currently supported Vendor Recipes
I do not intend to support all quality recipes, but I might add new in the future. Currently supported recipes:
- [Glassblower's Bauble](https://pathofexile.gamepedia.com/Glassblower%27s_Bauble)
- [Gemcutter's Prism](https://pathofexile.gamepedia.com/Gemcutter%27s_Prism)

> Note: The tool only cares about the "sum of multiple items qualities is at least 40%" vendor recipe. Single item of 20% quality recipe is ignored.

## Usage
Download the tool [from here](https://github.com/TehGM/PoE-QualityRecipesCalculator/releases).

After downloading the app, run it through your CMD/Terminal.

```bash
QualityRecipesCalculator -s <POESESSID> -a <AccountName> -l <League> -r <Platform>
```

There are a few arguments to provide to the application:
- `-s <POESESSIID>` - *Required* - Your PoE [Session ID](http://www.vhpg.com/how-to-find-poe-session-id/).
- `-a <AccountName>` - *Required* - Your PoE Account Name
- `-l <League>` - *Optional* - The League to check stashes for, like `Ritual`, `SSF Ritual`, `Hardcore Ritual`, `Standard` etc. Defaults to `Standard`.
- `-r <Platform>` - *Optional* - The platform you play on. Valid values are `pc`, `xbox` and `sony`. Defaults to `pc`.
- `--only-exact` - *Optional* - By default, the app outputs combinations that have total 40% quality or more. Adding this flag makes the app output __only__ the combinations that have total quality equal __exactly__ 40%.
- `--show-invalid` - *Optional* - By default, the app will not output combinations that have total quality below 40%. Adding this flag makes the app output them as well.
- `--item-names` - *Optional* - By default, the app will only show only the quality of each item in combination. Adding this flag will output the example item names as well for easier finding. *Warning: it might make the app output less readable*.
- `--debug` - *Optional* - Makes the app output debug logs.

##### Tip!
You can also run `QualityRecipesCalculator --help` to list all supported switches and their default values.

## License
Copyright (c) 2021 TehGM 

Licensed under [Mozilla Public License 2.0](LICENSE).