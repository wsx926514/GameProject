﻿using System.Globalization;
using Unity.Hierarchy;
using UnityEngine;

public class Constants
{
    public static string STORY_PATH = "story";
    public static string DEFAULT_STORY_FILE_NAME = "1";
    public static string EXCEL_FILE_EXTENSION = ".xlsx";
    public static int DEFAULT_START_LINE = 1;

    public static string AVATAR_PATH = "image/avatar/";
    public static string BACKGROUND_PATH = "image/background/";
    public static string BUTTON_PATH = "image/button/";
    public static string CHARACTER_PATH = "image/character/";
    public static string THUMBNAIL_PATH = "image/thumbnail/";
    public static string IMAGE_LOAD_FAILED = "Failed to load image: ";
    public static string BIG_IMAGE_LOAD_FAILED = "Failed to load big image: ";

    public static float DEFAULT_TYPING_SPEED = 0.2f; 
    public static float SKIP_MODE_TYPING_SPEED = 0.01f;

    public static string AUTO_ON = "auto on";
    public static string AUTO_OFF = "auto stop";
    public static float DEFAULT_AUTO_WAITING_SECONDS = 0.1f;

    public static string SKIP_ON = "skip on";
    public static string SKIP_OFF = "skip off";
    public static float DEFAULT_SKIP_WAITING_SECONDS = 0.02f;



    public static string VOCAL_PATH = "audio/vocal/";
    public static string MUSIC_PATH = "audio/music/";
    public static string AUDIO_LOAD_FAILED = "Failed to load audio: ";

    public static string NO_DATA_FOUND = "No data found";
    public static string END_OF_STORY = "End of story: ";
    public static string CHOICE = "choice";
    public static float DEFAULT_WAITING_SECONDS = 0.005f;

    public static string APPEAR_AT = "appearAt";
    public static string APPEAR_AT_INSTANTLY = "appearAtInstantly";
    public static string DISAPPEAR = "disappear";
    public static string MOVE_TO = "moveTo";
    public static int DURATION_TIME = 1;
    public static string COORDINATE_MISSING = "Coordinate missing  ";



    public static int DEFAULT_START_INDEX = 0;
    public static int SLOTS_PER_PAGE = 8;
    public static int TOTAL_SLOTS = 40;
    public static string COLON = ": ";
    public static string SAVE_GAME = "save_game";
    public static string LOAD_GAME = "load_game";
    public static string EMPLY_SLOT = "empty_slot";

    public static string CAMERA_NOT_FOUND = "Main camera not found!";
    public static string SAVE_FILE_PATH = "saves";
    public static string SAVE_FILE_EXTENSION = ".json";

    public static string X = "x";

    public static int MAX_LENGTH = 50;
    public static int GALLERY_SLOTS_PER_PAGE = 9;
    public static string GALLERY = "gallery";
    public static string GALLERY_PLACEHOLDER = "gallery_placeholder";
    public static readonly string[] ALL_BACKGROUNDS = { "1","2","3","4","5","6","bedroom" };//畫廊圖片名
    public static string UNLOCKED = "unlocked";

    public static string CONFIRM = "confirm";
    public static string PROMPT_TEXT = "enter_name";
    public static string NAME_PLACEHOLDER = "[Name]";

    public static string GOTO = "goto";
    public static string NEW_STORY_FILE_NAME = "11";

    public static string DEFAULT_LANGUAGE = "zh";
    public static string LANGUAGE_PATH = "languages";
    public static string JSON_FILE_EXTENSION = ".json";
    public static string LOCALIZATION_LOAD_FAILED = "Failed to load localization file: ";
    public static int DEFAULT_LANGUAGE_INDEX = 0;

    public static string CHINESE = "中文";
    public static string ENGLISH = "English";
    public static string JAPANESE = "日本語";
    public static string[] LANGUAGES = { "zh","en","ja" };

    public static string PREV_PAGE = "previous_page";
    public static string NEXT_PAGE = "next_page";
    public static string BACK = "back";
    public static string CLOSE = "close";

    public static string FULLSCREEN = "fullscreen ";
    public static string WINDOWED = "windowed ";
    public static string RESET = "reset";

}
