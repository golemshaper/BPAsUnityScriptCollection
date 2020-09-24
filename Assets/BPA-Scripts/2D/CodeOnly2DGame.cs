using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polygon2DGE
{
    public class CodeOnly2DGame : MonoBehaviour
    {
        public Texture2D tileSource;
        public Polygon2DRenderer polygon2DRenderer;
        public List<SpriteInteger> spriteList= new List<SpriteInteger>();
        public List<TileAnimation> animationsList= new List<TileAnimation>();
        Color invisible = new Color(0,0,0,0);

        //TODO: Tie these two integers together in a new class!
        int HeroID = -1;
        int HeroAnimID = -1;
        //----------------------------------------------------------
        private void Start()
        {
            CreatePlayer();
        }
        void CreatePlayer()
        {
            HeroID = CreateSprite(0, 0, 8, 0, 32);
            animationsList.Add(new TileAnimation());
            HeroAnimID = animationsList.Count - 1;
            animationsList[HeroAnimID].AddFrame(0,0);
            animationsList[HeroAnimID].AddFrame(1,0);
        }
        void PlayerLogic()
        {
            spriteList[HeroID].x += 24f * Time.deltaTime;
            //                  initial,range,  speed
            spriteList[HeroID].y = 64f + 8* Mathf.Sin(2*Time.time);
            animationsList[HeroAnimID].UpdateTick();
            spriteList[HeroID].SetNewTile(animationsList[HeroAnimID].GetTile());
        }
        private void Update()
        {
            PlayerLogic();
            polygon2DRenderer.ClearCanvas(invisible);
            DrawSprites(spriteList);
        }
        void DrawSprites(List<SpriteInteger> sprites)
        {
            for (int i = 0; i < sprites.Count; i++)
            {
               Spr(sprites[i]);
            }
        }
        public int CreateSprite(int tileX, int tileY, int nTileSize, int nX, int nY)
        {
            spriteList.Add(new SpriteInteger(tileX, tileY, 8, nX, nY));
            return spriteList.Count-1;
        }
        public void Spr(SpriteInteger spriteInt)
        {
            Spr(spriteInt.tileIDX,spriteInt.tileIDY,spriteInt.tileSize, 
                Mathf.RoundToInt(spriteInt.x), Mathf.RoundToInt(spriteInt.y));
        }
        public void Spr(int spriteID_X,int spriteID_Y,int tileSize, int x,int y)
        {
            polygon2DRenderer.SpriteBlit(tileSource, tileSize, spriteID_X, spriteID_Y,x,y,false,false);
        }
    }
    [System.Serializable]
    public class SpriteInteger
    {
        public SpriteInteger()
        {

        }
        public SpriteInteger(int tileX,int tileY,int nTileSize, int nX, int nY)
        {
            tileSize = nTileSize;
            tileIDX = tileX*tileSize;
            tileIDY = tileY*tileSize;
            x = nX;
            y = nY;
        }
        public void SetNewTile(TileFrame frame)
        {
            SetNewTile(frame.tileX,frame.tileY);
        }
        public void SetNewTile(int tileX, int tileY)
        {
            tileIDX = tileX*tileSize;
            tileIDY = tileY*tileSize;
        }
        public int tileIDX;
        public int tileIDY;
        public int tileSize = 8;
        //Maybe make these a vector 2
        public float x;
        public float y;
    }
    public class TileAnimation
    {
        
        int currentFrame = 0;
        float curTime = 0f;
        public List<TileFrame> frames = new List<TileFrame>();
        public void AddFrame(int nX , int nY)
        {
            frames.Add(new TileFrame(nX, nY));
        }
        public void AddFrame(int nX, int nY, float frameLength)
        {
            frames.Add(new TileFrame(nX, nY, frameLength));
        }
        public TileFrame GetTile()
        {
            return frames[currentFrame];
        }
        public int GetFrame()
        {
            return currentFrame;
        }
        public int UpdateTick()
        {
            if(curTime<frames[currentFrame].frameDuration)
            {
                curTime += Time.deltaTime;
                return GetFrame();
            }
            currentFrame++;
            curTime = 0f;
            if (currentFrame>=frames.Count)
            {
                currentFrame = 0;
            }
            return GetFrame();
        }
    }
    public class TileFrame
    {
        public TileFrame(int x, int y)
        {
            tileX = x;
            tileY = y;
        }
        public TileFrame(int x, int y, float frameTime)
        {
            tileX = x;
            tileY = y;
            frameDuration = frameTime;
        }
        public int tileX;
        public int tileY;
        public float frameDuration = 0.2f;
    }
}