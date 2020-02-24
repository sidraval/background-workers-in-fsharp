module Option
let Do predicate op =
    match op with
    | Some(v) -> v |> predicate
    | None -> ignore()
