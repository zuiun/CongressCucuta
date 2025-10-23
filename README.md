# Congress of Cúcuta

Government simulation game/slideshow program, made for nostalgic reasons.

## Installation

Congress of Cúcuta uses [.NET 9.0](https://dotnet.microsoft.com/en-us/download/dotnet/9.0/runtime).
Whenever 10.0 comes out, it will use that instead.
I have yet to create an `.exe`, as Congress of Cúcuta is not ready for release.

## Usage

Run the provided `.exe` (whenever it is released) and follow the directions.
Congress of Cúcuta requires a `.sim` file to run.
Several are available in this repository and can be found in the `CongressCucuta/Simulations` folder.

## Simulations

These simulations are provided with Congress of Cúcuta.
I made sure to convert at least one simulation from each government type for which I had written simulations.
In order of creation date:

<details>
<summary>
Colombia, Presidential Republic: The Campaigns of the South
</summary>
Simón Bolívar has just liberated Quito from Spanish rule and dreams of a continent-spanning federation centred around the nascent state of Colombia.
However, this "Great Colombia" project faces many enemies, and they are not merely external.
The departments are now engaged in a vitriolic power struggle for national control and direction that threatens the integrity of the state.

If the brewing conflict is not stopped, there may not even be a lesser Colombia in the coming years.

This was my first simulation; Congress of Cúcuta is named after its original premise.
It is by far the least detailed and least interesting simulation, but I like looking back on it as the one that started everything.
</details>

<details>
<summary>
Poland, Parliamentary Republic: Parliamentocracy
</summary>
After more than a century of partition, Poland is finally independent.
However, there is no time to celebrate.
Ethnically and politically diverse, the Poles' experiences under occupation have shaped their attitudes toward Poland's future, and cooperation is not among those attitudes.
Terrorism, embargoes, and the threat of invasion are ever-present, and nobody seems to have a solution.

Poland is not yet lost, but nothing lasts forever.
</details>

<details>
<summary>
Japan, Feudal Monarchy: The Northern and Southern Courts
</summary>
For the first time since Japan's founding, the emperor wields temporal power.
This has not gone unchallenged by the traditional military establishment, and a low-level civil war continues.
Takauji Ashikaga, an upstart general, was instrumental in Daigo II's restoration, but has since turned on his master.
Realistically, the Ashikagas have already won this war.
All that remains is the conflict between the nobility and the warriors for legitimate power.

The question is, how can a minor warrior family maintain control over a massive, fractious empire?
</details>

<details>
<summary>
China, Oligarchic Dictatorship: The Nanking Decade
</summary>
With the end of the Northern Expedition, China is finally a united republic, but there is no time to rest.
Revolution in Russia has inspired China's first communist movement, while endemic warlordism continues to frustrate efforts at reform.
Dr. Yat-sen Sun will never live to see his revolution come to fruition.

Hopefully, his heirs will be up to this monumental task.
</details>

<details>
<summary>
Indonesia, Oligarchic Dictatorship: Guided Democracy
</summary>
Indonesia is a patchwork of ethnicities, religions, and races, held together by a common colonial past.
Unfortunately, that past is not enough, and those frustrated with legislative gridlock have instead turned to armed rebellion.
In a last-ditch effort to preserve order, Sukarno has established a dictatorship with himself at the helm.

Perhaps the guiding hand of Indonesia's founding father will be enough to ensure prosperity for all time.
</details>

<details>
<summary>
Hungary, Elective Dictatorship: The Horthy Regency
</summary>
The end of the Great War and the so-called Trianon Trauma have brought Hungary to the edge of despair.
Miklós Horthy, an admiral without a fleet, now rules over a kingdom without a king, and the Anglophile regent has since been spurned by the British.
All across Hungary, cries of "No, no, never!" and "Return everything!" echo in the halls of power, but Horthy remains cautious.

After all, once this twenty-year armistice ends, Hungary could still lose everything.
</details>

<details>
<summary>
Argentina, Military Dictatorship: The Argentine Revolution
</summary>
The political scene in Argentina has never been stable, and the newly elected president, Arturo Illia, does not inspire much confidence.
Soft-hearted, humble, and conciliatory, Illia may have been the right candidate for a state with strong democratic traditions, but he is not the right candidate for the Argentine military.
A small clique of leading officers has overthrown Illia, promising to ward away the Peronist spectre and revive the faltering economy.

Only time will tell if these inexperienced officers will succeed.
</details>

<details>
<summary>
Malaysia, Elective Monarchy: Malaysian Malaysia
</summary>
Formed as a federation between several British colonies, Malaysia was beset from her inception by racial conflict and foreign infiltration.
Having just expelled Chinese-majority Singapore from the union, many question whether Malaysia will ever be a state for all Malaysians, or if it will become a state for only the Malays.

Hopefully, the spectre of an all-consuming race war remains confined to the imagination.
</details>

<details>
<summary>
Australia, Constitutional Monarchy: The Whitlam Era
</summary>
Twenty-three years of continuous Liberal-Country rule have just come to an end.
With inflation and unemployment at record levels, voters head to the polls with reform on their minds.
The result: charismatic Labor leader Gough Whitlam is Australia's newest Prime Minister.
His programme is radical, extensive, and, above all, exciting.

It's time for a change.
</details>

<details>
<summary>
Canada, Constitutional Monarchy: Patriation
</summary>
Canada has just adopted the iconic Maple Leaf flag, but this did not occur without controversy.
The Anglophone and Francophone communities, which see each other as having distinct ethnonational origins, are not united in their love for Canada.
The Quiet Revolution in Quebec has led to an outpouring of Quebecer nationalism, and the two nations of Canada seem to be at a crossroads.

Canada marches onwards towards independence, but she may not come out in one piece.
</details>

<details>
<summary>
Finland, Mixed Republic: Finlandisation
</summary>
The Second World War has not left Finland unscathed.
Bound by a treaty with the USSR, Finnish leaders have very little in the way of foreign policy independence and dare not accept American support.
For some, this is alright; anything to avoid another war.

But if Finland should ever become independent, then her leaders have no choice but to poke the bear.
</details>

<details>
<summary>
Brazil, Presidential Republic: The Old Republic
</summary>
For the first time, Brazil is without a monarch.
For the leaders of the new republic, this is a dream come true.
For nearly everyone else, this is a disaster.
Without the old emperor at the helm, Brazil's politicians now face the challenge of garnering public support and uniting the many states of Brazil.

In time, they will understand why Pedro II was so eager to part with his poisoned inheritance.
</details>

## Creation

If you can decipher the `.sim` files, then you could theoretically write a simulation from scratch.
However, realistically, you should download the source code and make some changes in your preferred C#/.NET editor:
- Create an `ISimulation` of your own.
Some examples are in the `CongressCucuta/Simulations` folder.
- `CongressCucuta/App.xaml` contains the entry point for the program.
This needs to be changed to `Views/CompilerWindow.xaml`.
- `CongressCucuta/ViewModels/CompilerViewModel.cs` stores compilable simulations.
Add your `ISimulation` to the `_simulations` field in the constructor.
- Build and run the project. The program will ask you to choose a file to save as `.sim`.
    - If the build fails, then this is likely an issue with your `ISimulation`.
    `Simulation` is an extremely rigid data structure that has several constraints.
    Ensure that you meet all of them before trying again.
    - If the build succeeds, then follow the directions to compile a `.sim`.
	You can then run your `.sim` with the main program.
