using UnityEngine;


public class ScreenShotter : MonoBehaviour  
{
    /* �Ĥ@�B: ����ù���e�ѪR�פؤo Screen.width�M Screen.height�OUnity���Ѫ��ݩʡA�\��O������ù��ѪR��
    �o�̻ݭn����]�O�]���I�Ϫ����z�j�p�ݭn�P�ù��ѪR�׬ۦP�A�o�ˤ~��T�O�I�Ϫ��M���שM����ʡC */
    public Texture2D CaptureScreenshot()
    {
        int width = Screen.width;
        int height = Screen.height;

        /*�ĤG�B:�Ыش�V���z
        �Ыؤ@���{�ɪ�RenderTexture(��V���z)
        �ѼƸѪR:
        width�M height�O�ù����e�שM���סA����ù��ؤo�C
        24:���`�׽w�İϪ���ơA�o�̳]�m��24��A��ܨϥ�24��`�׽w�İϡA�q�`�����ϥΡC
        RenderTexture�O�@�دS���z�A�Ω��x�s��۾�����V��X
        ²�Ө����i�H�z�Ѭ��۾����e��(Canvas)*/

        RenderTexture rt = RenderTexture.GetTemporary(width, height, 24);

        /*�ĤT�B:�ˬd�D�۾��O�_�s�b
        �@�άO�T�O���������D�۾��¦b(Camera.main �O Unity���Ѫ��ֳt����D�۾����ݩ�)�C
        ������ݭn�ˬd�D�۾��O�_�s�b�H
        �p�G�D�۾����s�b�A��V�ާ@�h�L�k����
        �o��N�X�O�q�LDebug.LogError()��k��X���~�H���챱��x�ê�^null�A����{�ǱY��C*/
        Camera mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main camera not found!");
            return null;
        }
        /*�ĥ|�B:�]�m�۾�����V�ؼ�
        ������ݭn�o�Ǿާ@�H
        �q�{���p�U�AUnity���۾��|�N��V���G��X��ù��W�C
        ���b�o�̡A�ڭ̻ݭn�N��V���G��X��RenderTexture���A�H�K����B�z�C
        �o��N�X�N�D�۾�����V�ؼг]�m�����Ыت�RenderTexture(rt)�A�o�ˬ۾���V�����e�N�|�Q��X��o��RenderTexture���C*/
        mainCamera.targetTexture = rt; //�i�D�D�۾��N��V���G��X��rt(��V���z)��
        RenderTexture.active = rt; //�N��V���z�]�m����e���ʪ���V�ؼСA�o�˫��򪺴�V�ާ@�N�|��X��o��RenderTexture���C
        mainCamera.Render(); //�j��D�۾��ߧY��V�����ÿ�X��rt

        /*�Ĥ��B: Ū�������ƾ�
        �ѼƸѪR:
        new Rect(0, 0, width, height)�O�Ψөw�q�@�ӯx�ΰϰ�A�o�ӯx�ΰϰ쪺���U�����Ь�(0, 0)�A�q���U���}�l�AŪ����ӿù��C
        0, 0: �N�����ƾڼg�J��ؼ�Texture2D����l��m
        ������ݭnApply()��k�H
        Texture2D����藍�|�ߧY�ͮġA�����ե�Apply()��A�ק�~�|��ڧ�s�쯾�z*/
        Texture2D screenShot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenShot.Apply();

        /*�Ĥ��B:���s��V�ؼШåB����귽*/
        mainCamera.targetTexture = null; //�N�D�۾�����V�ؼг]�m��null�A�o�˥D�۾��N�|��_���q�{����V�欰�A�Y�N��V���G��X��ù��W�C
        RenderTexture.active = null; //�M����e�Ұʪ���V���z�A�o�˥i�H�קK���򪺴�V�ާ@�v�T��o��RenderTexture�C
        RenderTexture.ReleaseTemporary(rt); //���񤧫e�Ыت�RenderTexture�귽�A�o�˥i�H�קK�O����n�|�C

        /*�ĤC�B: �Y�p�I��
         �ե�ResizeTexture ��k�A�N�I���Y�p���Ӫ�1/6�ؤo
        �Y�p���z�ѬO�A�Y�p�I�ϥi�H��ֹϤ����j�p�A�`�٦s�x�Ŷ��M�ǿ�a�e�C
        �q�`�Ω�O�E�C�����Ҫ��Y���ϩΪ̵o�e�����ШD*/
        Texture2D resizedScreenshot = ResizeTexture(screenShot, width / 6, height / 6);

        /*�ĤK�B: �P����l�I�ϡA����O����*/
        Destroy(screenShot);
        return resizedScreenshot;
    }

    /*���U��k:�N��J��Texture2D�Y�p����w���e�שM����
     ��^��: ��^�Y�p�᪺Texture2D��H*/
    private Texture2D ResizeTexture(Texture2D original, int newWidth, int newHeight)
    {
        /*�Ĥ@�B:�Ыش�V���z
        �Ыؤ@�ӻP�ؼиѪR�׬ۦP����V���z�A�ñҰʥ�*/
        RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight, 24);
        RenderTexture.active = rt;

        /*�ĤG�B:�ϥ�GPU�Y��
         * �ϥ�GPU��Graphics.Blit�Noriginal�������ƾڽƻs���Y���rt
        ������ϥ�GPU?
        GPU�ާ@���ʳv�����Y��󰪮ġA�A�X�Y�ɾާ@*/
        Graphics.Blit(original, rt);

        /*�ĤT�B:Ū���Y��᪺�ƾ�
        Ū��RederTexture�����e��s��Texture2D��H���A�P�I�Ϫ��޿�����*/
        Texture2D resized = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
        resized.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
        resized.Apply();

        /*�ĥ|�B:�M�z�귽
        �M�z�{�ɪ�RenderTexture�귽�A�קK�O����n�|*/
        RenderTexture.active = null; //�M����e���ʪ���V���z
        RenderTexture.ReleaseTemporary(rt); //���񤧫e�Ыت�RenderTexture�귽

        return resized; //��^�Y�p�᪺Texture2D��H
    }







}
