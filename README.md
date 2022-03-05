# TileBased-MapGenerator

<div>
    <img src="/Media/Screenshot%20Small.PNG" alt="Small" style="float: left; width: 33%; margin-right: 1%; margin-bottom: 0.5em;">
    <img src="/Media/Screenshot%20Medium.PNG" alt="Medium"style="float: left; width: 33%; margin-right: 1%; margin-bottom: 0.5em;">
    <img src="/Media/Screenshot%20Large.PNG" alt="Large" style="float: left; width: 33%; margin-right: 1%; margin-bottom: 0.5em;">
</div>

The map generator generates maps in two passes using `TileGenerator`s and `TileIndexer`s.

## Tile Generators
The first pass places the tiles onto the tile map via `TileGenerator`s. The tile map has one layer per tile type, where each layer is a 2D array containing a flag for whether a given cell in the map contains a tile of that type or not. `TileGenerator`s are applied to the tile map one-by-one. Each `TileGenerator` works based on the tile map that the previous `TileGenerator`s have collectively generated to add (or remove) tiles to a single layer.
There are tile generators that generate tiles based on perlin noise, find paths between other tiles or remove tiles based on the given rules.

<p align="center">
  <img  align="center" src="/Media/Documentation/Doc_TileGenerator.png" alt="Example Tile Generation" style="width: 70%;"/>
</p>

The above figure illustrates how tiles are being generated: The first layer places a grass tile (light green) in each cell. The second and third layers place water tiles (blue)  and forest tiles (dark green) on top of the grass layer. Tile generators can be configured to be blocked by tiles that have been placed previously. For instance, the tile generator for forest tiles is allowed to place forest tiles into cells that contain a grass tile, but is blocked from placing tiles in cells containing a water tile.

## Tile Indexers
The second pass determines which sprite of a sprite sheet is rendered. Similarly to `TileGenerator`s this can happen in various ways. For some tiles, such as the grass and forest tiles, one random sprite is chosen out of the sprite sheet in each cell. For terrain tiles, like water the surrounding 8 tiles are checked and depending on those, a sprite is chosen out of the sprite sheet.

<p align="center">
  <img  align="center" src="/Media/Documentation/Doc_TileIndexer.PNG" alt="Example Tile Generation" style="width: 70%;"/>
</p>

The above figure shows how the tile map is rendered. Each layer is processed by a `Tile Indexer`, which determines the sprite which is to be drawn.


## Generation of the Road Network

<p align="center">
  <img  align="center" src="/Media/Documentation/Doc_RoadNetwork.png" alt="Example Tile Generation" style="width: 70%;"/>
</p>

The towns are randomly placed on the map via the *random grid* algorithm of the [2D Point Scattering](http://www.tom.ille-web.de/PointDistribution/index.html) project.

To build up the road network, the tile map is processed into a tree that is traversable by an A* algorithm. Each cell in the tile map corrensponds to a node with connections to each non-diagonal neighbor.
The different terrains are weighted differently. Nodes that represent cells with grassland have low costs, whereas forest and mountains have higher costs. Water is not traversable. This generates more interesting and natural roads. 
The `TileGenerator` responsible for the road generation finds all possible pairs of neighboring towns and orders them ascendingly by their distance. It then runs through all neighbor pairs and generates a path via A*. It updates its internal tree with the newly added path. It significantly reduces the costs of nodes that the path passes through. For the next neighbor pairs, the A* algorithm therefore prefers the path tiles wherever possible which generates a network of interconnected road rather than a set of independant roads. If the `TileGenerator` tries to connect two towns that have already been connected it finds the shortes path along previously created roads and does not update the tile map.
After all neighbors have been connected the roads are made more visually clear, by removing all terrain tiles that are in cells with road or town tiles. This can be observed in the difference between the seconds and third image in the above figure.

## 

<br>
<p align="center">
  <img src="/Media/Animation%20Medium.gif"  style="width: 70%;">
</p>
