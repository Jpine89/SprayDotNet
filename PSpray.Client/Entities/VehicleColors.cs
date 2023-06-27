using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSpray.Client.Entities
{
	internal class VehicleColors
	{
		public static Dictionary<int, ColorArray> VehiclePairs = new()
		{
			{1  , new ColorArray( 255, 255, 255 ) },    // NONE                          
			{2  , new ColorArray( 13, 17, 22 ) },       // METALLIC_BLACK                
			{3  , new ColorArray( 28, 28, 33 ) },       // METALLIC_GRAPHITE_BLACK       
			{4  , new ColorArray( 50, 56, 61 ) },       // METALLIC_BLACK_STEAL          
			{5  , new ColorArray( 69, 75, 79 ) },       // METALLIC_DARK_SILVER          
			{6  , new ColorArray( 153, 157, 160 ) },    // METALLIC_SILVER               
			{7  , new ColorArray( 194, 196, 198 ) },    // METALLIC_BLUE_SILVER          
			{8  , new ColorArray( 151, 154, 151 ) },    // METALLIC_STEEL_GRAY           
			{9  , new ColorArray( 99, 115, 128 ) },     // METALLIC_SHADOW_SILVER        
			{10  , new ColorArray( 99, 98, 92 ) },       // METALLIC_STONE_SILVER         
			{11  , new ColorArray( 60, 63, 71 ) },      // METALLIC_MIDNIGHT_SILVER      
			{12  , new ColorArray( 68, 78, 84 ) },      // METALLIC_GUN_METAL            
			{13  , new ColorArray( 29, 33, 41 ) },      // METALLIC_ANTHRACITE_GREY      
			{14  , new ColorArray( 19, 24, 31 ) },      // MATTE_BLACK                   
			{15  , new ColorArray( 38, 40, 42 ) },      // MATTE_GRAY                    
			{16  , new ColorArray( 81, 85, 84 ) },      // MATTE_LIGHT_GREY              
			{17  , new ColorArray( 21, 25, 33 ) },      // UTIL_BLACK                    
			{18  , new ColorArray( 30, 36, 41 ) },      // UTIL_BLACK_POLY               
			{19  , new ColorArray( 51, 58, 60 ) },      // UTIL_DARK_SILVER              
			{20  , new ColorArray( 140, 144, 149 ) },   // UTIL_SILVER                   
			{21  , new ColorArray( 57, 67, 77 ) },      // UTIL_GUN_METAL                
			{22  , new ColorArray( 80, 98, 114 ) },     // UTIL_SHADOW_SILVER            
			{23  , new ColorArray( 30, 35, 47 ) },      // WORN_BLACK                    
			{24  , new ColorArray( 54, 58, 63 ) },      // WORN_GRAPHITE                 
			{25  , new ColorArray( 160, 161, 153 ) },   // WORN_SILVER_GREY              
			{26  , new ColorArray( 211, 211, 211 ) },   // WORN_SILVER                   
			{27  , new ColorArray( 183, 191, 202 ) },   // WORN_BLUE_SILVER              
			{28  , new ColorArray( 119, 135, 148 ) },   // WORN_SHADOW_SILVER            
			{29  , new ColorArray( 192, 14, 26 ) },     // METALLIC_RED                  
			{30  , new ColorArray( 218, 25, 24 ) },     // METALLIC_TORINO_RED           
			{31  , new ColorArray( 182, 17, 27 ) },     // METALLIC_FORMULA_RED          
			{32  , new ColorArray( 165, 30, 35 ) },     // METALLIC_BLAZE_RED            
			{33  , new ColorArray( 123, 26, 34 ) },     // METALLIC_GRACEFUL_RED         
			{34  , new ColorArray( 142, 27, 31 ) },     // METALLIC_GARNET_RED           
			{35  , new ColorArray( 111, 24, 24 ) },     // METALLIC_DESERT_RED           
			{36  , new ColorArray( 73, 17, 29 ) },      // METALLIC_CABERNET_RED         
			{37  , new ColorArray( 182, 15, 37 ) },     // METALLIC_CANDY_RED            
			{38  , new ColorArray( 212, 74, 23 ) },     // METALLIC_SUNRISE_ORANGE       
			{39  , new ColorArray( 194, 148, 79 ) },    // METALLIC_CLASSIC_GOLD         
			{40  , new ColorArray( 247, 134, 22 ) },    // METALLIC_ORANGE               
			{41  , new ColorArray( 207, 31, 33 ) },     // MATTE_RED                     
			{42  , new ColorArray( 115, 32, 33 ) },     // MATTE_DARK_RED                
			{43  , new ColorArray( 242, 125, 32 ) },    // MATTE_ORANGE                  
			{44  , new ColorArray( 255, 201, 31 ) },    // MATTE_YELLOW                  
			{45  , new ColorArray( 156, 16, 22 ) },     // UTIL_RED                      
			{46  , new ColorArray( 222, 15, 24 ) },     // UTIL_BRIGHT_RED               
			{47  , new ColorArray( 143, 30, 23 ) },     // UTIL_GARNET_RED               
			{48  , new ColorArray( 169, 71, 68 ) },     // WORN_RED                      
			{49  , new ColorArray( 177, 108, 81 ) },    // WORN_GOLDEN_RED               
			{50  , new ColorArray( 55, 28, 37 ) },      // WORN_DARK_RED                 
			{51  , new ColorArray( 19, 36, 40 ) },      // METALLIC_DARK_GREEN           
			{52  , new ColorArray( 18, 46, 43 ) },      // METALLIC_RACING_GREEN         
			{53  , new ColorArray( 18, 56, 60 ) },      // METALLIC_SEA_GREEN            
			{54  , new ColorArray( 49, 66, 63 ) },      // METALLIC_OLIVE_GREEN          
			{55  , new ColorArray( 21, 92, 45 ) },      // METALLIC_GREEN                
			{56  , new ColorArray( 27, 103, 112 ) },    // METALLIC_GASOLINE_BLUE_GREEN  
			{57  , new ColorArray( 102, 184, 31 ) },    // MATTE_LIME_GREEN              
			{58  , new ColorArray( 34, 56, 62 ) },      // UTIL_DARK_GREEN               
			{59  , new ColorArray( 29, 90, 63 ) },      // UTIL_GREEN                    
			{60  , new ColorArray( 45, 66, 63 ) },      // WORN_DARK_GREEN               
			{61  , new ColorArray( 69, 89, 75 ) },      // WORN_GREEN                    
			{62  , new ColorArray( 101, 134, 127 ) },   // WORN_SEA_WASH                 
			{63  , new ColorArray( 34, 46, 70 ) },      // METALLIC_MIDNIGHT_BLUE        
			{64  , new ColorArray( 35, 49, 85 ) },      // METALLIC_DARK_BLUE            
			{65  , new ColorArray( 48, 76, 126 ) },     // METALLIC_SAXONY_BLUE          
			{66  , new ColorArray( 71, 87, 143 ) },     // METALLIC_BLUE                 
			{67  , new ColorArray( 99, 123, 167 ) },    // METALLIC_MARINER_BLUE         
			{68  , new ColorArray( 57, 71, 98 ) },      // METALLIC_HARBOR_BLUE          
			{69  , new ColorArray( 214, 231, 241 ) },   // METALLIC_DIAMOND_BLUE         
			{70  , new ColorArray( 118, 175, 190 ) },   // METALLIC_SURF_BLUE            
			{71  , new ColorArray( 52, 94, 114 ) },     // METALLIC_NAUTICAL_BLUE        
			{72  , new ColorArray( 11, 156, 241 ) },    // METALLIC_BRIGHT_BLUE          
			{73  , new ColorArray( 47, 45, 82 ) },      // METALLIC_PURPLE_BLUE          
			{74  , new ColorArray( 40, 44, 77 ) },      // METALLIC_SPINNAKER_BLUE       
			{75  , new ColorArray( 35, 84, 161 ) },     // METALLIC_ULTRA_BLUE           
			{76  , new ColorArray( 110, 163, 198 ) },   // METALLIC_BRIGHT_BLUE2         
			{77  , new ColorArray( 17, 37, 82 ) },      // UTIL_DARK_BLUE                
			{78  , new ColorArray( 27, 32, 62 ) },      // UTIL_MIDNIGHT_BLUE            
			{79  , new ColorArray( 39, 81, 144 ) },     // UTIL_BLUE                     
			{80  , new ColorArray( 96, 133, 146 ) },    // UTIL_SEA_FOAM_BLUE            
			{81  , new ColorArray( 36, 70, 168 ) },     // UTIL_LIGHTNING_BLUE           
			{82  , new ColorArray( 66, 113, 225 ) },    // UTIL_MAUI_BLUE_POLY           
			{83  , new ColorArray( 59, 57, 224 ) },     // UTIL_BRIGHT_BLUE              
			{84  , new ColorArray( 31, 40, 82 ) },      // MATTE_DARK_BLUE               
			{85  , new ColorArray( 37, 58, 167 ) },     // MATTE_BLUE                    
			{86  , new ColorArray( 28, 53, 81 ) },      // MATTE_MIDNIGHT_BLUE           
			{87  , new ColorArray( 76, 95, 129 ) },     // WORN_DARK_BLUE                
			{88  , new ColorArray( 88, 104, 142 ) },    // WORN_BLUE                     
			{89  , new ColorArray( 116, 181, 216 ) },   // WORN_LIGHT_BLUE               
			{90  , new ColorArray( 255, 207, 32 ) },    // METALLIC_TAXI_YELLOW          
			{91  , new ColorArray( 251, 226, 18 ) },    // METALLIC_RACE_YELLOW          
			{92  , new ColorArray( 145, 101, 50 ) },    // METALLIC_BRONZE               
			{93  , new ColorArray( 224, 225, 61 ) },    // METALLIC_YELLOW_BIRD          
			{94  , new ColorArray( 152, 210, 35 ) },    // METALLIC_LIME                 
			{95  , new ColorArray( 155, 140, 120 ) },   // METALLIC_CHAMPAGNE            
			{96  , new ColorArray( 80, 50, 24 ) },      // METALLIC_PUEBLO_BEIGE         
			{97  , new ColorArray( 71, 63, 43 ) },      // METALLIC_DARK_IVORY           
			{98  , new ColorArray( 34, 27, 25 ) },      // METALLIC_CHOCO_BROWN          
			{99  , new ColorArray( 101, 63, 35 ) },     // METALLIC_GOLDEN_BROWN         
			{100  , new ColorArray( 119, 92, 62 ) },     // METALLIC_LIGHT_BROWN          
			{101  , new ColorArray( 172, 153, 117 ) },  // METALLIC_STRAW_BEIGE          
			{102  , new ColorArray( 108, 107, 75 ) },   // METALLIC_MOSS_BROWN           
			{103  , new ColorArray( 64, 46, 43 ) },     // METALLIC_BISTON_BROWN         
			{104  , new ColorArray( 164, 150, 95 ) },   // METALLIC_BEECHWOOD            
			{105  , new ColorArray( 70, 35, 26 ) },     // METALLIC_DARK_BEECHWOOD       
			{106  , new ColorArray( 117, 43, 25 ) },    // METALLIC_CHOCO_ORANGE         
			{107  , new ColorArray( 191, 174, 123 ) },  // METALLIC_BEACH_SAND           
			{108  , new ColorArray( 223, 213, 178 ) },  // METALLIC_SUN_BLEECHED_SAND    
			{109  , new ColorArray( 247, 237, 213 ) },  // METALLIC_CREAM                
			{110  , new ColorArray( 58, 42, 27 ) },     // UTIL_BROWN                    
			{111  , new ColorArray( 120, 95, 51 ) },    // UTIL_MEDIUM_BROWN             
			{112  , new ColorArray( 181, 160, 121 ) },  // UTIL_LIGHT_BROWN              
			{113  , new ColorArray( 255, 255, 246 ) },  // METALLIC_WHITE                
			{114  , new ColorArray( 234, 234, 234 ) },  // METALLIC_FROST_WHITE          
			{115  , new ColorArray( 176, 171, 148 ) },  // WORN_HONEY_BEIGE              
			{116  , new ColorArray( 69, 56, 49 ) },     // WORN_BROWN                    
			{117  , new ColorArray( 42, 40, 43 ) },     // WORN_DARK_BROWN               
			{118  , new ColorArray( 114, 108, 87 ) },   // WORN_STRAW_BEIGE              
			{119  , new ColorArray( 106, 116, 124 ) },  // BRUSHED_STEEL                 
			{120  , new ColorArray( 53, 65, 88 ) },     // BRUSHED_BLACK_STEEL           
			{121  , new ColorArray( 155, 160, 168 ) },  // BRUSHED_ALUMINIUM             
			{122  , new ColorArray( 88, 112, 161 ) },   // CHROME                        
			{123  , new ColorArray( 234, 230, 222 ) },  // WORN_OFF_WHITE                
			{124  , new ColorArray( 223, 221, 208 ) },  // UTIL_OFF_WHITE                
			{125  , new ColorArray( 242, 173, 46 ) },   // WORN_ORANGE                   
			{126  , new ColorArray( 249, 164, 88 ) },   // WORN_LIGHT_ORANGE             
			{127  , new ColorArray( 131, 197, 102 ) },  // METALLIC_SECURICOR_GREEN      
			{128  , new ColorArray( 241, 204, 64 ) },   // WORN_TAXI_YELLOW              
			{129  , new ColorArray( 76, 195, 218 ) },   // POLICE_CAR_BLUE               
			{130  , new ColorArray( 78, 100, 67 ) },    // MATTE_GREEN                   
			{131  , new ColorArray( 188, 172, 143 ) },  // MATTE_BROWN                   
			{132  , new ColorArray( 248, 182, 88 ) },   // WORN_ORANGE2                  
			{133  , new ColorArray( 252, 249, 241 ) },  // MATTE_WHITE                   
			{134  , new ColorArray( 255, 255, 251 ) },  // WORN_WHITE                    
			{135  , new ColorArray( 129, 132, 76 ) },   // WORN_OLIVE_ARMY_GREEN         
			{136  , new ColorArray( 255, 255, 255 ) },  // PURE_WHITE                    
			{137  , new ColorArray( 242, 31, 153 ) },   // HOT_PINK                      
			{138  , new ColorArray( 253, 214, 205 ) },  // SALMON_PINK                   
			{139  , new ColorArray( 223, 88, 145 ) },   // METALLIC_VERMILLION_PINK      
			{140  , new ColorArray( 246, 174, 32 ) },   // ORANGE                        
			{141  , new ColorArray( 176, 238, 110 ) },  // GREEN                         
			{142  , new ColorArray( 8, 233, 250 ) },    // BLUE                          
			{143  , new ColorArray( 10, 12, 23 ) },     // METTALIC_BLACK_BLUE           
			{144  , new ColorArray( 12, 13, 24 ) },     // METALLIC_BLACK_PURPLE         
			{145  , new ColorArray( 14, 13, 20 ) },     // METALLIC_BLACK_RED            
			{146  , new ColorArray( 159, 158, 138 ) },  // HUNTER_GREEN                  
			{147  , new ColorArray( 98, 18, 118 ) },    // METALLIC_PURPLE               
			{148  , new ColorArray( 11, 20, 33 ) },     // METAILLIC_V_DARK_BLUE         
			{149  , new ColorArray( 17, 20, 26 ) },     // MODSHOP_BLACK1                
			{150  , new ColorArray( 107, 31, 123 ) },   // MATTE_PURPLE                  
			{151  , new ColorArray( 30, 29, 34 ) },     // MATTE_DARK_PURPLE             
			{152  , new ColorArray( 188, 25, 23 ) },    // METALLIC_LAVA_RED             
			{153  , new ColorArray( 45, 54, 42 ) },     // MATTE_FOREST_GREEN            
			{154  , new ColorArray( 105, 103, 72 ) },   // MATTE_OLIVE_DRAB              
			{155  , new ColorArray( 122, 108, 85 ) },   // MATTE_DESERT_BROWN            
			{156  , new ColorArray( 195, 180, 146 ) },  // MATTE_DESERT_TAN              
			{157  , new ColorArray( 90, 99, 82 ) },     // MATTE_FOILAGE_GREEN           
			{158  , new ColorArray( 129, 130, 127 ) },  // DEFAULT_ALLOY_COLOR           
			{159  , new ColorArray( 175, 214, 228 ) },  // EPSILON_BLUE                  
			{160  , new ColorArray( 122, 100, 64 ) },   // PURE_GOLD                     
			{161  , new ColorArray( 127, 106, 72 ) },   // BRUSHED_GOLD           
		};     

    }

	internal class ColorArray
	{
		public int R { get; set; }
		public int G { get; set; }
		public int B { get; set; }

		public ColorArray(int r, int g, int b)
        {
			R = r;
			G = g;
			B = b;
        }
	}
}


