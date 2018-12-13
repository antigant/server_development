public enum EvCode
{
    LOGIN = 0,
    REGISTRATION,
    LOGOUT,
    RESET_PASSWORD,
    UPDATE_ITEM,
    INIT_INVENTORY,
    ITEM_STATE, // Using this to receive message from plugin to turn off the active state
}