namespace Pathfinding.General {
    public class SearchNode {
        public Node NearestNodeToStart { get; set; }
        public float MinCostToStart { get; set; }
        public bool WasVisited { get; set; }

        public SearchNode() {
            NearestNodeToStart = null;
            MinCostToStart = float.MaxValue;
            WasVisited = false;
        }
    }
}
