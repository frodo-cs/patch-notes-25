using Player.Puzzles;

namespace Player.Gameplay.ClickableItems
{
    public class FreightDoor : Door
    {

        protected override void Start()
        {
            base.Start();
            Simon.OnSimonComplete += OpenDoor;
        }

        private void OpenDoor()
        {
            isDoorOpen = true;
        }

        private void OnDestroy()
        {
            Simon.OnSimonComplete -= OpenDoor;
        }

    }
}
