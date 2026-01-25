using UnityEngine;

public class DataPlayerStat : MonoBehaviour
{
    public interface IPlayerStat
    {
        void cLuck(int luck);
        void cStreng(int streng);
        void cDefence(int defence);
        void cSpeed(int speed);
        void cHealth(int health);
    }
}
