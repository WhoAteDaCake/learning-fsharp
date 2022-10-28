module Client.Layout.Header

open Client.Conversion
open Client
open Client.Routing
open Fable.Builders.AntDesign
open Fable.Core

let view (url: Url) =
    let items =
        {| key = "test"; label = "hello" |}

    toReact
    <| JSX.jsx
        $"""
        import {Menu} from 'antd';
            <Menu
            theme="dark"
            mode="horizontal"
            defaultSelectedKeys={[ '2' ]}
            items={items}
      />
      """
// Header {
//     Row {
//         Menu {
//             mode MenuMode.Horizontal
//             selectedKeys [|(urlToPath url)|]
//             topLevelRoutes
//             |> List.map (fun x -> )
//         }
//     }
// }
