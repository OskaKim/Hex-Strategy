using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UITop : MonoBehaviour
{
    enum LensType {
        None,
        Environment,
        Feature,
        Continent,
        Climate
    }

    public void OnSelectLensDropDownElement(int index) {
        var lensType = (LensType)index;

        switch (lensType) {
            case LensType.None:
                Tile.TileHelper.ClearHexMesh();
                break;
            case LensType.Environment:
                Tile.TileHelper.SetTilesColorToEnvironment();
                break;
            case LensType.Feature:
                Tile.TileHelper.SetTilesColorToFeatureType();
                break;
            case LensType.Continent:
                Tile.TileHelper.SetTilesColorToContinent();
                break;
            case LensType.Climate:
                Tile.TileHelper.SetTilesColorToClimate();
                break;
            default:
                Debug.LogError("UI동작이 정의되지 않았습니다");
                break;
        }
    }
    
    public void OnClickButtonReCreateMap() {
        var tileCreater = Tile.TileCreater.GetInstance();
        tileCreater.ReCreateMap();
    }
}
