using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Level : MonoBehaviour
{
    const float PIPE_WIDTH = 7.8f;
    const float PIPE_HEAD_HEIGHT = 3.75f;
    const float CAMERA_ORTHO_SIZE = 50f;
    const float PIPE_MOVE_SPEED = 50f;
    const float PIPE_DESTROY_XPOSITION = -126f;
    const float PIPE_SPAWN_XPOSITION = 126f;
    private static Level instance;
    List<Pipe> pipeList;
    public Text scoreText;
    int pipeSpawned;
    int pipesPassedCount;
    float pipeSpawnTimer;
    float pipeSpawnTimerMax;
    float gapSize;
    const float BIRD_X_POSITION=0f;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible,
    }
    public static Level GetInstance(){
      return instance;
    }
    public int GetPipesSpawned(){
      return pipeSpawned;
    }

    void Awake()
    {
        instance=this;
        pipesPassedCount=0;
        pipeList = new List<Pipe>();
        pipeSpawnTimerMax = 1f;
        SetDifficulty(Difficulty.Easy);
    }

    void Start()
    {
        //CreateGapPipes(50f, 20f, 20f);
    }

    void Update()
    {
        HandlePipeMovenment();
        HandlePipeSpawning();
        HandleUI();
    }

    void HandleUI(){
      scoreText.text=GetPipesPassedCount().ToString();      
    }

    void SetDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                gapSize = 50;
                pipeSpawnTimerMax = 1.2f;
                break;
            case Difficulty.Medium:
                gapSize = 40;
                pipeSpawnTimerMax = 1.1f;
                break;
            case Difficulty.Hard:
                gapSize = 33;
                pipeSpawnTimerMax = 1.0f;
                break;
            case Difficulty.Impossible:
                gapSize = 24;
                pipeSpawnTimerMax = 0.8f;
                break;
        }
    }

    public int GetPipesPassedCount(){
      return pipesPassedCount/2;
    }

    Difficulty GetDifficulty()
    {

        if (pipeSpawned >= 30)
            return Difficulty.Impossible;
        if (pipeSpawned >= 20)
            return Difficulty.Hard;
        if (pipeSpawned >= 10)
            return Difficulty.Medium;
        return Difficulty.Easy;
    }

    void HandlePipeSpawning()
    {
        pipeSpawnTimer -= Time.deltaTime;

        if (pipeSpawnTimer < 0)
        {
            pipeSpawnTimer += pipeSpawnTimerMax;

            float heightEdgeLimit = 10f;
            float minHeight = gapSize / 2 + heightEdgeLimit;
            float totalHeight = CAMERA_ORTHO_SIZE * 2;
            float maxHeight = totalHeight - gapSize / 2 - heightEdgeLimit;

            float height = Random.Range(minHeight, maxHeight);

            CreateGapPipes(height, gapSize, PIPE_SPAWN_XPOSITION);
        }
    }

    void HandlePipeMovenment()
    {
        Pipe pipe;
        for (int i = 0; i < pipeList.Count; i++)
        {
            pipe = pipeList[i];
            bool isToTheRightOfBird=pipe.GetXPosition()>BIRD_X_POSITION;
            pipe.Move();

            if (isToTheRightOfBird&&pipe.GetXPosition()<=BIRD_X_POSITION){
              pipesPassedCount+=1;
            }
            if (pipe.GetXPosition() < PIPE_DESTROY_XPOSITION)
            {
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    void CreateGapPipes(float gapY, float gapSize, float xPosition)
    {
        CreatePipe(gapY - gapSize / 2, xPosition, true);
        CreatePipe(CAMERA_ORTHO_SIZE * 2f - gapY - gapSize / 2, xPosition, false);
        pipeSpawned++;
        SetDifficulty(GetDifficulty());
    }
    void CreatePipe(float height,float xPosition, bool createBottom)
    {
        Transform pipeHead = Instantiate(GameAssets.GetInstance().pfPipeHead);
        float pipeHeadYPosition;
        if (createBottom)
        {
            pipeHeadYPosition = -CAMERA_ORTHO_SIZE + height - PIPE_HEAD_HEIGHT * 0.48f;
        }
        else
        {
            pipeHeadYPosition = +CAMERA_ORTHO_SIZE - height + PIPE_HEAD_HEIGHT * 0.48f;
        }
        pipeHead.position = new Vector3(xPosition, pipeHeadYPosition);
        //pipeList.Add(pipeHead);

        Transform pipeBody = Instantiate(GameAssets.GetInstance().pfPipeBody);
        float pipeBodyYPosition;
        if (createBottom)
        {
            pipeBodyYPosition = -CAMERA_ORTHO_SIZE + height / 2;
        }
        else
        {
            pipeBodyYPosition = +CAMERA_ORTHO_SIZE - height / 2;
        }
        pipeBody.position = new Vector3(xPosition, pipeBodyYPosition);
        //pipeList.Add(pipeBody);

        SpriteRenderer pipeBodySpriteRenderer = pipeBody.GetComponent<SpriteRenderer>();
        pipeBodySpriteRenderer.size = new Vector2(PIPE_WIDTH, height);

        BoxCollider2D pipeBodyBoxCollider = pipeBody.GetComponent<BoxCollider2D>();
        pipeBodyBoxCollider.size = new Vector2(PIPE_WIDTH, height);

        Pipe pipe = new Pipe(pipeHead, pipeBody);
        pipeList.Add(pipe);
    }

    class Pipe
    {
        Transform pipeHeadTransform;
        Transform pipeBodyTransform;
        public Pipe(Transform pipeHeadTransform, Transform pipeBodyTransform)
        {
            this.pipeBodyTransform = pipeBodyTransform;
            this.pipeHeadTransform = pipeHeadTransform;
        }

        public void Move()
        {
            pipeHeadTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
            pipeBodyTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;
        }

        public float GetXPosition()
        {
            return pipeHeadTransform.position.x;
        }
        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }

}
