# 100 Days Of Code - Log


### Day 1: January 9, Monday 

#### Today's Progress
1. Forked [100-days-of-code](https://github.com/Kallaway/100-days-of-code)
2. Updated docs of fork with my own info
3. Copied previous progress over from [unity-vr-puppeteer repo](https://github.com/bjennings76/unity-vr-puppeteer)
4. Cleaned up [Favro Board](https://favro.com/organization/2857ca4e59648312669b3470/4f3ad4ae8deea00fd4a99356) and made it public

#### Thoughts
It's nice to be back from vacation and get back to sweet, sweet code. Mostly cleanup and preparation for this hour.

#### Link(s) to work
Steps: [Join the #100DaysOfCode](https://medium.freecodecamp.com/join-the-100daysofcode-556ddb4579e4#.962oiqjiq)


### Day 2: January 10, Tuesday

#### Today's Progress
1. Created a spinner script to turn the character selector
2. Cleaned up a lot of files
3. Upgraded to VRTK 3.0 and fixed issues created by upgrade

#### Thoughts
It was a rough start due to some VRTK upgrade issues, but once I got the table spinner working, things got fun. I threw together some puppets onto it to test it out. They turn well and throw their little limbs around with inertial forces making the turn table fun to play with. It definitely feels like the right way to for puppet selection.

Next up: Getting the puppets to cycle through all options as the spinner turns.

#### Link(s) to work
[Spinning Spinner](https://github.com/bjennings76/unity-vr-puppeteer/blob/master/Captures/Day%20002%20-%20Spinner.gif)


### Day 3: January 11, Wednesday

#### Today's Progress
1. Fixed all Unity 5.5 obsolete call warnings from the build created by SteamVR and ProBuilder
2. Created a 'magic box' inside which items can get swapped out
3. Started work on creating a list of object creators.

#### Thoughts
Ran in to an interesting problem where I would like to have bespoke object prefabs that can be instanced along with a list of objects created dynamically from a single object through sub-object changes (swapping costumes on a single character, for instance). This forced me to think out exactly how to create a list of objects that don't yet exist. I'm exploring the idea of a list of item creators which would have a default creator for prefab version and customized create functions for the dynamic objects. Wasn't able to finish it up, but I should have a list of object creators to cycle through tomorrow.

#### Link(s) to work
[Changing Room Capture](https://github.com/bjennings76/unity-vr-puppeteer/blob/master/Captures/Day%20003%20-%20Magic%20Changing%20Room.gif)


### Day 4: January 12, Wednesday

#### Today's Progress
1. Puppets now are dynamically placed onto the spinner.
2. Discovered how `Math.Mod()` can be used to wrap an index into a valid index in a list. e.g. `index = index % count + count) % count;`

#### Thoughts
Most of the work is under the hood, but it'll be pretty slick to use going forward. I'm especially proud of keeping the item generation classes small and legible.


### Day 5: January 11, Wednesday

#### Today's Progress
1. Upgraded to SteamVR 1.2
2. Moved the project back into [unity-vr-puppeteer](https://github.com/bjennings76/unity-vr-puppeteer) so work shows up in GitHub contributions
3. Item Carousel now properly swaps out character models as they go through the changing room.

#### Thoughts
Nice to see some visible progress today and was happy to be able to capture the end result.

#### Link(s) to work
[Item Carousel swapping models](https://github.com/bjennings76/unity-vr-puppeteer/blob/master/Captures/Day%20005%20-%20Puppet%20Carousel%20Swapping%20Models.gif)

<!-- Template


### Day 3: January 11, Wednesday

#### Today's Progress
1. 

#### Thoughts

#### Link(s) to work
-->