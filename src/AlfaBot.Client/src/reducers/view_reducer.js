import {CHANGE_TAB} from "../actions/types";

const initialState = {
    activeTab: "/results",
};

export default (state = initialState, {type, payload}) => {
    switch (type) {
        case CHANGE_TAB:
            return {
                ...state,
                activeTab: payload,
            };

        default:
            return state;
    }
};