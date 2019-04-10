import * as actions from "../actions/types";

const initialState = {
    token: null
};

export default (state = initialState, {type, payload}) => {
    switch (type) {
        case actions.TOKEN_SET:
            return {
                token: payload
            };

        case actions.TOKEN_INVALIDATE:
            return {
                token: null
            };

        default:
            return state;
    }
};
