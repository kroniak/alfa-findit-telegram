import React, {Component} from "react";
import Notification from "arui-feather/notification";

export default class CustomNotification extends Component {
    constructor(props) {
        super(props);

        this.state = {
            notifyVisible: false
        }
    }

    componentWillReceiveProps(nextProps, nextContext) {
        if (nextProps.error && nextProps.error !== this.props.error) this.setState({
            notifyVisible: true
        })
    }

    render() {
        const {title = "Ошибка загрузки", message = "Что то произошло не так!"} = this.props;
        return <Notification
            visible={this.state.notifyVisible}
            status='error'
            offset={100}
            stickTo='right'
            title={title}
            onCloseTimeout={() => {
                this.setState({notifyVisible: false});
            }}
            onCloserClick={() => {
                this.setState({notifyVisible: false});
            }}>
            {message}
        </Notification>
    }
}