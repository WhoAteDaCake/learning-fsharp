module Client.AppLayout.AppHeader

open Client.Routing
open Feliz

let view (url: Url) = Html.div [ prop.text "Header" ]
//     Header {
//         Row {
//             Menu {
//                 mode MenuMode.Horizontal
//                 selectedKeys [| (urlToPath url) |]
//                 Children
//             // topLevelRoutes |> List.map (fun x -> str "hello")
//             }
//         }
//     }
