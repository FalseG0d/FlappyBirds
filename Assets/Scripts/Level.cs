using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey;
using CodeMonkey.Utils;

public class Level : MonoBehaviour
{
    const float PIPE_WIDTH = 7.8f;
    const float PIPE_HEAD_HEIGHT = 3.75f;
    const float CAMERA_ORTHO_SIZE = 50f;
    const float PIPE_MOVE_SPEED = 50f;
    const float PIPE_DESTROY_XPOSITION = -126f;
    const float PIPE_SPAWN_XPOSITION = 126f;
    const float GROUND_DESTROY_XPOSITION = -236f;
    const float GROUND_SPAWN_X_POSITION = 126f;
    private static Level instance;
    public GameObject gameOver;
    List<Pipe> pipeList;
    List<Transform> groundList;
    public Text gameOverScoreText;
    public Text scoreText;
    public Text highScoreText;
    int pipeSpawned;
    int pipesPassedCount;
    float pipeSpawnTimer;
    float pipeSpawnTimerMax;
    float gapSize;
    State state;
    const float BIRD_X_POSITION=0f;

    public enum Difficulty
    {
        Easy,
        Medium,
        Hard,
        Impossible,
    }
    enum State{
      Waiting,
      Alive,
      Dead
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
        state=State.Waiting;
        Hide();
    }

    void Start()
    {
        //CreateGapPipes(50f, 20f, 20f);
        highScoreText.text="Highscore: "+Score.GetHighScore().ToString();
        Bird.GetInstance().OnDeath+=Bird_OnDied;
        Bird.GetInstance().OnStart+=Bird_OnStartPlaying;
    }
    void Bird_OnStartPlaying(object sender,System.EventArgs e){
      state=State.Alive;
    }
    void Bird_OnDied(object sender,System.EventArgs e){
      //throw new System.NotImplementedException();
      //Debug.Log("Dead");
      Score.SetHighScore(Level.GetInstance().GetPipesPassedCount());
      state=State.Dead;
      int score=Level.GetInstance().GetPipesPassedCount();
      if(score>=Score.GetHighScore()){
        gameOverScoreText.text="New Highscore!!\n"+(score).ToString();
      }else{
        gameOverScoreText.text=(score).ToString();
      }
      Show();
    }

    void Update()
    {
        if(state==State.Alive){
          HandlePipeMovenment();
          HandlePipeSpawning();
          HandleUI();
          SpawnInitialGround();
          HandleGround();
      }
    }

    void SpawnInitialGround(){
      groundList=new List<Transform>();
      Transform groundTransform;
      float groundY=-44f;
      float groundWidth=230f;
      groundTransform=Instantiate(GameAssets.GetInstance().pfGround,new Vector3(0,groundY,0),Quaternion.identity);
      groundList.Add(groundTransform);
      groundTransform=Instantiate(GameAssets.GetInstance().pfGround,new Vector3(groundWidth,groundY,0),Quaternion.identity);
      groundList.Add(groundTransform);
      groundTransform=Instantiate(GameAssets.GetInstance().pfGround,new Vector3(groundWidth*2,groundY,0),Quaternion.identity);
      groundList.Add(groundTransform);
    }

    private void HandleGround() {
        foreach (Transform groundTransform in groundList) {
            groundTransform.position += new Vector3(-1, 0, 0) * PIPE_MOVE_SPEED * Time.deltaTime;

            if (groundTransform.position.x < GROUND_DESTROY_XPOSITION) {
                // Ground passed the left side, relocate on right side
                // Find right most X position
                float rightMostXPosition = -100f;
                for (int i = 0; i < groundList.Count; i++) {
                    if (groundList[i].position.x > rightMostXPosition) {
                        rightMostXPosition = groundList[i].position.x;
                    }
                }

                // Place Ground on the right most position
                float groundWidth = 236f;
                groundTransform.position = new Vector3(rightMostXPosition + groundWidth, groundTransform.position.y, groundTransform.position.z);
            }
        }
    }

    void HandleUI(){
      scoreText.text=(GetPipesPassedCount()).ToString();
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

            if (isToTheRightOfBird&&pipe.GetXPosition()<BIRD_X_POSITION){
              pipesPassedCount+=1;
              SoundManager.PlaySound(SoundManager.Sound.BirdScore);
            }
            if (pipe.GetXPosition() < PIPE_DESTROY_XPOSITION)
            {
                pipe.DestroySelf();
                pipeList.Remove(pipe);
                i--;
            }
        }
    }

    void Hide(){
      gameOver.SetActive(false);
    }
    void Show(){
      gameOver.SetActive(true);
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
            return pipeHeadTransform.position.x+5;
        }
        public void DestroySelf()
        {
            Destroy(pipeHeadTransform.gameObject);
            Destroy(pipeBodyTransform.gameObject);
        }
    }

}
