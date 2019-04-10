import {combineReducers} from "redux";

import results from "./results_reducer";
import users from "./users_reducer";
import token from "./token_reducer";

export default combineReducers({
    results,
    users,
    token
});
